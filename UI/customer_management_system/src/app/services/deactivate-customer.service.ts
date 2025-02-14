import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { GenericResponse } from '../interfaces/generic-response';
import { GetCustomersService } from './get-customers.service';
import { HttpHeaderService } from './http-header-service';

@Injectable({
  providedIn: 'root',
})
export class DeactivateCustomerService {
  readonly APIURL =
    environment.CustomerManagementSystemAPI + '/api/Customer/deactivate';

  constructor(
    private http: HttpClient,
    private httpHeaderService: HttpHeaderService,
    private getCustomersService: GetCustomersService, // Inject GetCustomersService to update locally
  ) {}

  deactivateCustomer(customerGUID: string): void {
    const headers: HttpHeaders =
      this.httpHeaderService.getHeadersWithTokenSet();
    const params = new HttpParams().set('customerGUID', customerGUID);

    this.http
      .patch<GenericResponse<object>>(this.APIURL, null, { headers, params })
      .subscribe({
        next: () => {
          // Update only the deactivated customer in-memory
          this.getCustomersService.updateCustomerLocally({
            guid: customerGUID,
            isActive: false, // Assuming there's an `isActive` property
          } as any);
        },
        error: (error) => console.error('Deactivation failed:', error),
      });
  }
}
