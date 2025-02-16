import { Component, OnInit, computed, Signal } from '@angular/core';
import { GetCustomerService } from '../../../../src/app/services/get-customer.service';
import { Customer } from '../../../../src/app/interfaces/get-customer-list-response';
import { Router } from '@angular/router';
import { ActivateCustomerService } from '../../services/activate-customer.service';
import { DeleteCustomerService } from '../../services/delete-customer.service';

@Component({
  selector: 'app-customer-list',
  templateUrl: './customer-list.component.html',
  styleUrls: ['./customer-list.component.css'],
})
export class CustomerListComponent implements OnInit {
  readonly customers;
  readonly isLoading;
  readonly errorMessage;

  // Computed signal to check for duplicate GUIDs
  readonly duplicateGuids = computed(() => {
    const guids = this.customers().map((customer) => customer.guid);
    return guids.filter((guid, index) => guids.indexOf(guid) !== index);
  });

  constructor(
    private readonly getCustomerService: GetCustomerService,
    private readonly activateCustomerService: ActivateCustomerService,
    private readonly deleteCustomerService: DeleteCustomerService,
    private readonly router: Router,
  ) {
    this.customers = this.getCustomerService.customersSignal;
    this.isLoading = this.getCustomerService.loadingSignal;
    this.errorMessage = this.getCustomerService.errorSignal;
  }

  ngOnInit(): void {
    this.getCustomerService.loadCustomers();

    if (this.duplicateGuids().length > 0) {
      console.warn('Duplicate GUIDs found:', this.duplicateGuids());
    } else {
      console.log('No duplicated GUIDs found');
    }
  }

  trackByCustomerId(index: number, customer: Customer): string {
    return customer.guid;
  }

  onGuidClick(customer: Customer): void {
    this.router.navigate(['/customerDetails'], {
      queryParams: { id: customer.guid },
    });
  }

  onEditClick(customer: Customer): void {
    this.router.navigate(['/editCustomer'], {
      queryParams: { id: customer.guid },
    });
  }

  onDeactivateClick(customer: Customer): void {
    this.activateCustomerService.deactivateCustomer(customer.guid);
  }

  onDeleteClick(customer: Customer): void {
    this.deleteCustomerService.deleteCustomer(customer.guid);
  }

  onReactivateClick(customer: Customer): void {
    this.activateCustomerService.reactivateCustomer(customer.guid);
  }
}
