import {
  HttpClient,
  HttpErrorResponse,
  HttpHeaders,
  HttpParams,
} from '@angular/common/http';
import { Injectable, Signal, signal } from '@angular/core';
import { Customer } from '../../../src/app/interfaces/get-customer-list-response';
import { environment } from '../../environments/environment';
import { GenericResponse } from '../interfaces/generic-response';
import { HttpHeaderService } from './http-header-service';

@Injectable({
  providedIn: 'root',
})
export class GetCustomerService {
  readonly APIURL =
    environment.CustomerManagementSystemAPI + '/api/Customer/get';

  private customer = signal<Customer | null>(null);
  private loading = signal<boolean>(false);
  private errorMessage = signal<string | null>(null);

  constructor(
    private http: HttpClient,
    private httpHeaderService: HttpHeaderService,
  ) {}

  getCustomer(queryString: string): void {
    this.loading.set(true);
    this.errorMessage.set(null);

    const headers: HttpHeaders =
      this.httpHeaderService.getHeadersWithTokenSet();
    const params = new HttpParams().set('searchVariable', queryString);

    this.http
      .get<GenericResponse<Customer>>(this.APIURL, { headers, params })
      .subscribe({
        next: (response) => {
          this.customer.set(response?.data ?? null);
          this.loading.set(false);
        },
        error: (error: HttpErrorResponse) => {
          this.handleError(error);
        },
      });
  }

  getCustomerSignal(): Signal<Customer | null> {
    return this.customer;
  }

  isLoading(): Signal<boolean> {
    return this.loading;
  }

  getErrorMessage(): Signal<string | null> {
    return this.errorMessage;
  }

  private handleError(error: HttpErrorResponse): void {
    console.error('Error fetching customer:', error);
    this.errorMessage.set(error.message || 'Server error');
    this.loading.set(false);
  }

  updateCustomerLocally(updatedCustomer: Customer): void {
    const existingCustomer = this.customer();
    if (existingCustomer && existingCustomer.guid === updatedCustomer.guid) {
      this.customer.set(updatedCustomer);
    }
  }
}
