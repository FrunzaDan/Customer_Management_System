// customer-list.component.ts
import { Component, OnInit, computed, effect, signal, Signal, inject, ChangeDetectionStrategy } from '@angular/core';
import { Router } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { GetCustomerService } from '../../services/get-customer.service';
import { ActivateCustomerService } from '../../services/activate-customer.service';
import { DeleteCustomerService } from '../../services/delete-customer.service';
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

  // Add CustomerStatus enum for better type checking
  readonly CustomerStatus = CustomerActivationStatus;

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

  ngOnInit(): void {
    this.getCustomerService.loadCustomers();
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
      next: () => this.deleting.set(false),
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
