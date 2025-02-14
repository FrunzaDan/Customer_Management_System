import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { GenericResponse } from '../../../src/app/interfaces/generic-response';
import { Customer } from '../../../src/app/interfaces/get-customer-list-response';
import { HttpHeaderService } from './http-header-service';
import { GetCustomerService } from './get-customer.service'; // Inject to update locally

@Injectable({
  providedIn: 'root',
})
export class EditCustomerService {
  private readonly APIURL =
    environment.CustomerManagementSystemAPI + '/api/Customer/edit';

  constructor(
    private http: HttpClient,
    private httpHeaderService: HttpHeaderService,
    private getCustomerService: GetCustomerService, // Used for local updates
  ) {}

  editCustomer(customer: Customer): void {
    const headers: HttpHeaders =
      this.httpHeaderService.getHeadersWithTokenSet();

    this.http
      .patch<GenericResponse<object>>(this.APIURL, customer, { headers })
      .subscribe({
        next: () => {
          // Update the local cache of the customer
          this.getCustomerService.updateCustomerLocally(customer);
        },
        error: (error) => console.error('Customer edit failed:', error),
      });
  }
}
