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
  styleUrls: ['./customer-list.component.css']
})
export class CustomerListComponent {

  constructor(private getCustomersService: GetCustomersService, private router: Router) {
  }
  getCustomerListResponse!: GetCustomerListResponse;
  customerList!: Customer[];

  ngOnInit(): void {
    this.getCustomersService.refreshTable().subscribe({
      next: response => {
        this.getCustomerListResponse = response;
        this.customerList = this.getCustomerListResponse.customerList;
      },
      error: error => console.log(error)
    })
  }

  onGuidClick(customer: Customer) {
    this.router.navigate(
      ['/customerDetails'],
      { queryParams: { id: customer.guid } }
    );
  }

  onEditClick(customer: Customer) {
    this.router.navigate(
      ['/editCustomer'],
      { queryParams: { id: customer.guid } }
    );
  }

  onDeactivateClick(customer: Customer) {
    this.router.navigate(
      ['/deactivateCustomer'],
      { queryParams: { id: customer.guid } }
    );
  }

}
