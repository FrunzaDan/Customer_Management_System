import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { GenericResponse } from '../interfaces/generic-response';
import { GetCustomerService } from './get-customer.service';
import { HttpHeaderService } from './http-header-service';

@Injectable({
  providedIn: 'root',
})
export class ActivateCustomerService {
  readonly APIURL_DEACTIVATE =
    environment.CustomerManagementSystemAPI + '/api/Customer/deactivate';
  readonly APIURL_REACTIVATE =
    environment.CustomerManagementSystemAPI + '/api/Customer/reactivate';

  constructor(
    private http: HttpClient,
    private httpHeaderService: HttpHeaderService,
    private getCustomerService: GetCustomerService, // Inject GetCustomersService to update locally
  ) {}

  deactivateCustomer(customerGUID: string): void {
    const headers: HttpHeaders =
      this.httpHeaderService.getHeadersWithTokenSet();
    const params = new HttpParams().set('customerGUID', customerGUID);

    this.http
      .patch<GenericResponse<object>>(this.APIURL_DEACTIVATE, null, {
        headers,
        params,
      })
      .subscribe({
        next: () => {
          const existingCustomer = this.getCustomerService
            .customersSignal()
            .find((c) => c.guid === customerGUID);

          if (existingCustomer) {
            this.getCustomerService.updateCustomerLocally({
              ...existingCustomer,
              customerStatus: 1903, // Setting status to deactivated
            });
          } else {
            console.warn(
              `Customer with GUID ${customerGUID} not found locally.`,
            );
          }
        },
        error: (error) => console.error('Activation task failed:', error),
      });
  }

  reactivateCustomer(customerGUID: string): void {
    const headers: HttpHeaders =
      this.httpHeaderService.getHeadersWithTokenSet();
    const params = new HttpParams().set('customerGUID', customerGUID);

    this.http
      .patch<GenericResponse<object>>(this.APIURL_REACTIVATE, null, {
        headers,
        params,
      })
      .subscribe({
        next: () => {
          const existingCustomer = this.getCustomerService
            .customersSignal()
            .find((c) => c.guid === customerGUID);

          if (existingCustomer) {
            this.getCustomerService.updateCustomerLocally({
              ...existingCustomer,
              customerStatus: 1901, // Setting status to reactivated
            });
          } else {
            console.warn(
              `Customer with GUID ${customerGUID} not found locally.`,
            );
          }
        },
        error: (error) => console.error('Activation task failed:', error),
      });
  }
}
