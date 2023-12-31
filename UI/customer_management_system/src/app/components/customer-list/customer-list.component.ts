import { Component, ElementRef, QueryList, ViewChild } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { HttpHeaders } from '@angular/common/http';
import { GetCustomersService } from 'src/app/services/get-customers.service';
import { GetCustomerListResponse } from 'src/app/interfaces/get-customer-list-response';
import { Customer } from 'src/app/interfaces/get-customer-list-response';
import { Router } from '@angular/router';

@Component({
  selector: 'app-customer-list',
  templateUrl: './customer-list.component.html',
  styleUrls: ['./customer-list.component.css'],
})
export class CustomerListComponent {
  getCustomerListResponse!: GetCustomerListResponse;
  customerList!: Customer[];
  loadCompleted: boolean = false;

  constructor(
    private getCustomersService: GetCustomersService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadCompleted = false;
    this.getCustomersService.refreshTable().subscribe({
      next: (response) => {
        this.getCustomerListResponse = response;
        this.customerList = this.getCustomerListResponse.customerList;
        this.loadCompleted = true;
      },
      error: (error) => {
        let errorStatusCode = error.status;
        if (errorStatusCode == 403) {
          this.router.navigate(['']);
        } else if (errorStatusCode == 404) {
          this.loadCompleted = true;
        }
      },
      complete: () => {},
    });
  }

  onGuidClick(customer: Customer) {
    this.router.navigate(['/customerDetails'], {
      queryParams: { id: customer.guid },
    });
  }

  onEditClick(customer: Customer) {
    this.router.navigate(['/editCustomer'], {
      queryParams: { id: customer.guid },
    });
  }

  onDeactivateClick(customer: Customer) {}

  onReactivateClick(customer: Customer) {}
}
