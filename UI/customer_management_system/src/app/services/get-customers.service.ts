import {
  HttpClient,
  HttpErrorResponse,
  HttpHeaders,
} from '@angular/common/http';
import { Injectable, Signal, signal } from '@angular/core';
import { environment } from '../../environments/environment';
import { GenericResponse } from '../interfaces/generic-response';
import { Customer } from '../interfaces/get-customer-list-response';
import { HttpHeaderService } from './http-header-service';

@Injectable({
  providedIn: 'root',
})
export class GetCustomersService {
  private readonly APIURL =
    environment.CustomerManagementSystemAPI + '/api/Customer/all';

  private customers = signal<Customer[]>([]);
  private loading = signal<boolean>(false);
  private errorMessage = signal<string | null>(null);

  constructor(
    private http: HttpClient,
    private httpHeaderService: HttpHeaderService,
  ) {
    this.loadCustomers();
  }

  private loadCustomers(): void {
    this.loading.set(true);
    this.errorMessage.set(null);

    const headers: HttpHeaders =
      this.httpHeaderService.getHeadersWithTokenSet();

    this.http
      .get<GenericResponse<Customer[]>>(this.APIURL, { headers })
      .subscribe({
        next: (response) => {
          this.customers.set(response?.data ?? []);
          this.loading.set(false);
        },
        error: (error: HttpErrorResponse) => {
          this.handleError(error);
        },
      });
  }

  getCustomers(): Signal<Customer[]> {
    return this.customers;
  }

  isLoading(): Signal<boolean> {
    return this.loading;
  }

  getErrorMessage(): Signal<string | null> {
    return this.errorMessage;
  }

  // Modify only the relevant customer without refreshing all data
  updateCustomerLocally(updatedCustomer: Customer): void {
    this.customers.update((customers) =>
      customers.map((c) =>
        c.guid === updatedCustomer.guid ? updatedCustomer : c,
      ),
    );
  }

  removeCustomerLocally(customerGUID: string): void {
    this.customers.update((customers) =>
      customers.filter((c) => c.guid !== customerGUID),
    );
  }

  refreshTable(): void {
    this.loadCustomers();
  }

  private handleError(error: HttpErrorResponse): void {
    console.error('Error fetching customers:', error);
    this.errorMessage.set(error.message || 'Server error');
    this.loading.set(false);
  }
}
