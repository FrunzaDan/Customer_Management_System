import {
  HttpClient,
  HttpErrorResponse,
  HttpHeaders,
  HttpParams,
} from '@angular/common/http';
import { computed, Injectable, Signal, signal } from '@angular/core';
import { Customer } from '../interfaces/get-customer-list-response';
import { environment } from '../../environments/environment';
import { GenericResponse } from '../interfaces/generic-response';
import { HttpHeaderService } from './http-header-service';

@Injectable({
  providedIn: 'root',
})
export class GetCustomerService {
  private readonly APIURL_ALL = `${environment.CustomerManagementSystemAPI}/api/Customer/all`;
  private readonly APIURL_SINGLE = `${environment.CustomerManagementSystemAPI}/api/Customer/get`;

  private readonly state = signal({
    customers: [] as Customer[],
    selectedCustomer: null as Customer | null,
    loading: false,
    error: null as string | null,
  });

  // Computed signals
  public readonly customers = computed(() => this.state().customers);
  public readonly selectedCustomer = computed(
    () => this.state().selectedCustomer,
  );
  public readonly loading = computed(() => this.state().loading);
  public readonly error = computed(() => this.state().error);

  constructor(
    private http: HttpClient,
    private httpHeaderService: HttpHeaderService,
  ) {
    this.loadCustomers();
  }

  public loadCustomers(): void {
    this.setLoading(true);

    const headers = this.httpHeaderService.getHeadersWithTokenSet();

    this.http
      .get<GenericResponse<Customer[]>>(this.APIURL_ALL, { headers })
      .subscribe({
        next: (response) => {
          this.state.update((state) => ({
            ...state,
            customers: response?.data ?? [],
            loading: false,
            error: null,
          }));
        },
        error: (error: HttpErrorResponse) => this.handleError(error),
      });
  }

  getCustomer(queryString: string): void {
    this.setLoading(true);

    const headers = this.httpHeaderService.getHeadersWithTokenSet();
    const params = new HttpParams().set('searchVariable', queryString);

    this.http
      .get<GenericResponse<Customer>>(this.APIURL_SINGLE, { headers, params })
      .subscribe({
        next: (response) => {
          this.state.update((state) => ({
            ...state,
            selectedCustomer: response?.data ?? null,
            loading: false,
            error: null,
          }));
        },
        error: (error: HttpErrorResponse) => this.handleError(error),
      });
  }

  updateCustomerLocally(updatedCustomer: Customer): void {
    this.state.update((state) => ({
      ...state,
      customers: state.customers.map((c) =>
        c.guid === updatedCustomer.guid ? updatedCustomer : c,
      ),
      selectedCustomer:
        state.selectedCustomer?.guid === updatedCustomer.guid
          ? updatedCustomer
          : state.selectedCustomer,
    }));
  }

  removeCustomerLocally(customerGUID: string): void {
    this.state.update((state) => ({
      ...state,
      customers: state.customers.filter((c) => c.guid !== customerGUID),
      selectedCustomer:
        state.selectedCustomer?.guid === customerGUID
          ? null
          : state.selectedCustomer,
    }));
  }

  private setLoading(loading: boolean): void {
    this.state.update((state) => ({ ...state, loading, error: null }));
  }

  private handleError(error: HttpErrorResponse): void {
    console.error('Error fetching customer data:', error);
    this.state.update((state) => ({
      ...state,
      loading: false,
      error: error.message || 'Server error',
    }));
  }
}
