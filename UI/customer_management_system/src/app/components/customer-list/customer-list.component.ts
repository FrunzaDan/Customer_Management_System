import { Component, OnInit } from '@angular/core';
import { GetCustomersService } from '../../../../src/app/services/get-customers.service';
import { Customer } from '../../../../src/app/interfaces/get-customer-list-response';
import { Router } from '@angular/router';
import { DeactivateCustomerService } from '../../services/deactivate-customer.service';
import { DeleteCustomerService } from '../../services/delete-customer.service';

@Component({
  selector: 'app-customer-list',
  templateUrl: './customer-list.component.html',
  styleUrls: ['./customer-list.component.css'],
  imports: [],
})
export class CustomerListComponent implements OnInit {
  customerList: Customer[] = [];
  loadCompleted = false;

  constructor(
    private readonly getCustomersService: GetCustomersService,
    private readonly deactivateCustomerService: DeactivateCustomerService,
    private readonly deleteCustomerService: DeleteCustomerService,
    private readonly router: Router,
  ) {}

  ngOnInit(): void {
    this.loadCustomerList();
  }

  private loadCustomerList(): void {
    this.loadCompleted = false;
    this.getCustomersService.refreshTable().subscribe({
      next: (response) => {
        this.customerList = response?.data ?? [];
        this.loadCompleted = true;
      },
      error: (error) => this.handleError(error),
    });
  }

  private handleError(error: any): void {
    if (error.status === 403) {
      this.router.navigate(['']);
    } else if (error.status === 404) {
      this.loadCompleted = true;
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
    this.deactivateCustomerService.deactivateCustomer(customer.guid).subscribe({
      next: () => {
        this.loadCustomerList();
      },
      error: (error) => console.error('Deactivation failed:', error),
    });
  }

  onReactivateClick(customer: Customer): void {}

  onDeleteClick(customer: Customer): void {
    this.deleteCustomerService.deleteCustomer(customer.guid);
  }
}
