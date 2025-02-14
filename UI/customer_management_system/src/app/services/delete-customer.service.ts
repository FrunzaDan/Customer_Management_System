import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { GenericResponse } from '../interfaces/generic-response';
import { GetCustomersService } from './get-customers.service';
import { HttpHeaderService } from './http-header-service';

@Injectable({
  providedIn: 'root',
})
export class DeleteCustomerService {
  readonly APIURL =
    environment.CustomerManagementSystemAPI + '/api/Customer/delete';

  constructor(
    private http: HttpClient,
    private httpHeaderService: HttpHeaderService,
    private getCustomersService: GetCustomersService, // Inject GetCustomersService to update locally
  ) {}

  deleteCustomer(customerGUID: string): void {
    const headers: HttpHeaders =
      this.httpHeaderService.getHeadersWithTokenSet();
    const params = new HttpParams().set('customerGUID', customerGUID);

    this.http
      .delete<GenericResponse<object>>(this.APIURL, { headers, params })
      .subscribe({
        next: () => {
          // Remove the customer from the local signal
          this.getCustomersService.removeCustomerLocally(customerGUID);
        },
        error: (error) => console.error('Deletion failed:', error),
      });
  }
}
