import { Component } from '@angular/core';
import { GetCustomerService } from '../../../../src/app/services/get-customer.service';
import { Customer } from '../../../../src/app/interfaces/get-customer-list-response';
import { Router } from '@angular/router';
import { DeactivateCustomerService } from '../../services/deactivate-customer.service';
import { DeleteCustomerService } from '../../services/delete-customer.service';
import { Signal } from '@angular/core';

@Component({
  selector: 'app-customer-list',
  templateUrl: './customer-list.component.html',
  styleUrls: ['./customer-list.component.css'],
})
export class CustomerListComponent {
  readonly customers;
  readonly isLoading;
  readonly errorMessage;

  constructor(
    private readonly getCustomerService: GetCustomerService,
    private readonly deactivateCustomerService: DeactivateCustomerService,
    private readonly deleteCustomerService: DeleteCustomerService,
    private readonly router: Router,
  ) {
    this.customers = this.getCustomerService.customers;
    this.isLoading = this.getCustomerService.loading;
    this.errorMessage = this.getCustomerService.error;
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
    this.deactivateCustomerService.deactivateCustomer(customer.guid);
  }

  onDeleteClick(customer: Customer): void {
    this.deleteCustomerService.deleteCustomer(customer.guid);
  }

  onReactivateClick(customer: Customer): void {
    this.deleteCustomerService.deleteCustomer(customer.guid);
  }
}
