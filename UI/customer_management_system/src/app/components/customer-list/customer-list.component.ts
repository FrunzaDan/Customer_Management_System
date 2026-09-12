// customer-list.component.ts
import { Component, OnInit, computed, effect, signal, Signal, inject, ChangeDetectionStrategy } from '@angular/core';
import { Router } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { GetCustomerService } from '../../services/get-customer.service';
import { ActivateCustomerService } from '../../services/activate-customer.service';
import { DeleteCustomerService } from '../../services/delete-customer.service';
import { ExportCustomerService } from '../../services/export-customer.service';
import {
  Customer,
  CustomerActivationStatus,
} from '../../interfaces/customer-response';

@Component({
  selector: 'app-customer-list',
  templateUrl: './customer-list.component.html',
  changeDetection: ChangeDetectionStrategy.Eager,
  styleUrls: ['./customer-list.component.css'],
})
export class CustomerListComponent implements OnInit {
  // Use dependency injection with inject()
  private readonly getCustomerService = inject(GetCustomerService);
  private readonly activateCustomerService = inject(ActivateCustomerService);
  private readonly deleteCustomerService = inject(DeleteCustomerService);
  private readonly exportCustomerService = inject(ExportCustomerService);
  private readonly router = inject(Router);

  // Public signals for template
  readonly customers = this.getCustomerService.customersSignal;
  readonly isLoading = this.getCustomerService.loadingSignal;
  readonly errorMessage = this.getCustomerService.errorSignal;
  readonly activationLoading = this.activateCustomerService.loadingSignal;
  readonly activationError = this.activateCustomerService.errorSignal;

  // Delete is a separate action from deactivate/reactivate, so it gets its own
  // in-flight/error state rather than being folded into activationLoading/Error.
  readonly deleting = signal(false);
  readonly deleteError = signal<string | null>(null);

  // CSV export exports whatever the list is currently searching/sorted by,
  // not just the current page — see ExportCustomerService.
  readonly exportLoading = this.exportCustomerService.loadingSignal;
  readonly exportError = this.exportCustomerService.errorSignal;

  // Add CustomerStatus enum for better type checking
  readonly CustomerStatus = CustomerActivationStatus;

  // Search, sorting, and pagination are all server-side now: every change to
  // any of these re-fetches just the relevant page from the API rather than
  // filtering/sorting an already-loaded full list in memory (see
  // GetCustomerService.loadCustomers and usp_getCustomers).
  readonly searchTerm = signal('');
  readonly sortColumn = signal<'name' | 'email' | 'msisdn'>('name');
  readonly sortDirection = signal<'asc' | 'desc'>('asc');

  readonly pageSize = 10;
  readonly currentPage = signal(1);

  readonly totalItems = this.getCustomerService.totalItemsSignal;
  readonly totalPages = computed(() =>
    Math.max(1, Math.ceil(this.totalItems() / this.pageSize)),
  );

  // Debounced so typing doesn't fire an API call per keystroke — the search
  // used to be a synchronous in-memory filter, but now it's a network call.
  private searchDebounceTimer: ReturnType<typeof setTimeout> | undefined;
  private static readonly SEARCH_DEBOUNCE_MS = 300;

  // Computed signal for duplicate GUIDs
  readonly duplicateGuids = computed(() => {
    const customers = this.customers();
    const guidCount = new Map<string, number>();

    customers.forEach((customer) => {
      const count = guidCount.get(customer.guid) ?? 0;
      guidCount.set(customer.guid, count + 1);
    });

    return Array.from(guidCount.entries())
      .filter(([_, count]) => count > 1)
      .map(([guid]) => guid);
  });

  constructor() {
    // duplicateGuids() is a computed signal, so re-run this check whenever
    // it actually changes instead of only once, synchronously, right after
    // the (async) loadCustomers() call in ngOnInit.
    effect(() => {
      const duplicates = this.duplicateGuids();
      if (duplicates.length > 0) {
        console.warn('Duplicate GUIDs found:', duplicates);
      }
    });

  }

  onSearchInput(value: string): void {
    this.searchTerm.set(value);

    clearTimeout(this.searchDebounceTimer);
    this.searchDebounceTimer = setTimeout(() => {
      // A narrower search can make the current page go out of range (e.g.
      // you're on page 3, then a search narrows results to one page) —
      // snap back to page 1 on every new search term.
      this.currentPage.set(1);
      this.fetchCustomers();
    }, CustomerListComponent.SEARCH_DEBOUNCE_MS);
  }

  goToPage(page: number): void {
    const target = Math.min(Math.max(page, 1), this.totalPages());
    if (target === this.currentPage()) return;
    this.currentPage.set(target);
    this.fetchCustomers();
  }

  setSort(column: 'name' | 'email' | 'msisdn'): void {
    if (this.sortColumn() === column) {
      this.sortDirection.set(this.sortDirection() === 'asc' ? 'desc' : 'asc');
    } else {
      this.sortColumn.set(column);
      this.sortDirection.set('asc');
    }
    this.currentPage.set(1);
    this.fetchCustomers();
  }

  ngOnInit(): void {
    this.fetchCustomers();
  }

  exportCsv(): void {
    this.exportCustomerService.exportCustomers({
      searchTerm: this.searchTerm().trim() || undefined,
      sortColumn: this.sortColumn(),
      sortDirection: this.sortDirection(),
    });
  }

  private fetchCustomers(): void {
    this.getCustomerService.loadCustomers({
      pageNumber: this.currentPage(),
      pageSize: this.pageSize,
      searchTerm: this.searchTerm().trim() || undefined,
      sortColumn: this.sortColumn(),
      sortDirection: this.sortDirection(),
    });
  }

  // Add return type and improve type safety
  trackByCustomerId(_: number, customer: Customer): string {
    return customer.guid;
  }

  // Navigation methods
  navigateToCustomer(guid: string): void {
    this.router.navigate(['/customerDetails'], {
      queryParams: { id: guid },
    });
  }

  navigateToEdit(guid: string): void {
    this.router.navigate(['/editCustomer'], {
      queryParams: { id: guid },
    });
  }

  // Customer action methods
  deactivateCustomer(guid: string): void {
    if (!confirm('Are you sure you want to deactivate this customer?')) {
      return;
    }
    this.activateCustomerService.deactivateCustomer(guid);
  }

  reactivateCustomer(guid: string): void {
    this.activateCustomerService.reactivateCustomer(guid);
  }

  deleteCustomer(guid: string): void {
    if (
      !confirm(
        'Are you sure you want to permanently delete this customer? This cannot be undone.',
      )
    ) {
      return;
    }

    this.deleting.set(true);
    this.deleteError.set(null);

    this.deleteCustomerService.deleteCustomer(guid).subscribe({
      next: () => {
        this.deleting.set(false);
        // removeCustomerLocally() (called by DeleteCustomerService) only
        // drops the row from the in-memory page — totalItems/page count
        // would go stale without a real re-fetch of the current page.
        this.fetchCustomers();
      },
      error: (error: HttpErrorResponse) => {
        this.deleting.set(false);
        this.deleteError.set(this.extractErrorMessage(error));
      },
    });
  }

  private extractErrorMessage(error: HttpErrorResponse): string {
    if (error.status === 0) {
      return 'Could not reach the server. It may be offline, or your browser does not trust its security certificate.';
    }
    return (
      error.error?.responseMessage ??
      error.error?.message ??
      `Request failed (${error.status}). Please try again.`
    );
  }
}
