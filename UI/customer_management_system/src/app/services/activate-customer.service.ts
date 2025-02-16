import {
  HttpClient,
  HttpErrorResponse,
  HttpHeaders,
  HttpParams,
} from '@angular/common/http';
import { computed, Injectable, signal } from '@angular/core';
import { environment } from '../../environments/environment';
import { GenericResponse } from '../interfaces/generic-response';
import { GetCustomerService } from './get-customer.service';
import { HttpHeaderService } from './http-header-service';
import { CustomerActivationStatus } from '../interfaces/customer-response';
import { retry } from 'rxjs/internal/operators/retry';
import { catchError } from 'rxjs/internal/operators/catchError';

interface ActivationState {
  loading: boolean;
  error: string | null;
}

@Injectable({
  providedIn: 'root',
})
export class ActivateCustomerService {
  readonly APIURL_DEACTIVATE =
    environment.CustomerManagementSystemAPI + '/api/Customer/deactivate';
  readonly APIURL_REACTIVATE =
    environment.CustomerManagementSystemAPI + '/api/Customer/reactivate';

  private readonly state = signal<ActivationState>({
    loading: false,
    error: null,
  });

  public readonly loadingSignal = computed(() => this.state().loading);
  public readonly errorSignal = computed(() => this.state().error);

  constructor(
    private http: HttpClient,
    private httpHeaderService: HttpHeaderService,
    private getCustomerService: GetCustomerService, // Inject GetCustomersService to update locally
  ) {}

  deactivateCustomer(customerGUID: string): void {
    this.setLoading(true);
    const headers: HttpHeaders =
      this.httpHeaderService.getHeadersWithTokenSet();
    const params = new HttpParams().set('customerGUID', customerGUID);

    this.http
      .patch<GenericResponse<object>>(this.APIURL_DEACTIVATE, null, {
        headers,
        params,
      })
      .pipe(
        retry(3),
        catchError((error: HttpErrorResponse) => {
          this.handleError(error);
          throw error;
        }),
      )
      .subscribe({
        next: (response) => {
          if (response.status != 200) {
            this.handleError(new Error('Deactivation failed'));
            return;
          }

          const existingCustomer = this.getCustomerService
            .customersSignal()
            .find((c) => c.guid === customerGUID);

          if (existingCustomer) {
            this.getCustomerService.updateCustomerLocally({
              ...existingCustomer,
              customerStatus: CustomerActivationStatus.Deactivated,
            });
            this.clearError();
          } else {
            this.handleError(
              new Error(
                `Customer with GUID ${customerGUID} not found locally.`,
              ),
            );
          }
        },
        error: (error: HttpErrorResponse) => this.handleError(error),
      });
  }

  reactivateCustomer(customerGUID: string): void {
    this.setLoading(true);
    const headers: HttpHeaders =
      this.httpHeaderService.getHeadersWithTokenSet();
    const params = new HttpParams().set('customerGUID', customerGUID);

    this.http
      .patch<GenericResponse<object>>(this.APIURL_REACTIVATE, null, {
        headers,
        params,
      })
      .pipe(
        retry(3),
        catchError((error: HttpErrorResponse) => {
          this.handleError(error);
          throw error;
        }),
      )
      .subscribe({
        next: (response) => {
          if (response.status != 200) {
            this.handleError(new Error('Reactivation failed'));
            return;
          }

          const existingCustomer = this.getCustomerService
            .customersSignal()
            .find((c) => c.guid === customerGUID);

          if (existingCustomer) {
            this.getCustomerService.updateCustomerLocally({
              ...existingCustomer,
              customerStatus: CustomerActivationStatus.Active,
            });
            this.clearError();
          } else {
            this.handleError(
              new Error(
                `Customer with GUID ${customerGUID} not found locally.`,
              ),
            );
          }
        },
        error: (error: HttpErrorResponse) => this.handleError(error),
      });
  }

  private setLoading(loading: boolean): void {
    this.state.update((state) => ({
      ...state,
      loading,
    }));
  }

  private clearError(): void {
    this.state.update((state) => ({
      ...state,
      loading: false,
      error: null,
    }));
  }

  private handleError(error: HttpErrorResponse | Error): void {
    let errorMessage = 'An unknown error occurred';

    if (error instanceof HttpErrorResponse) {
      if (error.status === 0) {
        errorMessage = 'Network error - please check your connection.';
      } else if (error.status >= 400 && error.status < 500) {
        errorMessage = error.error?.message || 'Client-side error occurred.';
      } else if (error.status >= 500) {
        errorMessage = 'Server error - please try again later.';
      }
    } else {
      errorMessage = error.message;
    }

    console.error('Activation Service Error:', errorMessage);
    this.state.update((state) => ({
      ...state,
      loading: false,
      error: errorMessage,
    }));
  }
}
