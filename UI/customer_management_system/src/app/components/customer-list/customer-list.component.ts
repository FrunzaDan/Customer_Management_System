// customer-list.component.ts
import { Component, OnInit, computed, Signal, inject, ChangeDetectionStrategy } from '@angular/core';
import { Router } from '@angular/router';
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

  ngOnInit(): void {
    this.getCustomerService.loadCustomers();

    const duplicates = this.duplicateGuids();
    if (duplicates.length > 0) {
      console.warn('Duplicate GUIDs found:', duplicates);
    }
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
    this.deleteCustomerService.deleteCustomer(guid);
  }
}
