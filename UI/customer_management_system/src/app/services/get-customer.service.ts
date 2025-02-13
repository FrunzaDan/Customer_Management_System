import { Injectable } from '@angular/core';
import {
  HttpClient,
  HttpErrorResponse,
  HttpHeaders,
  HttpParams,
} from '@angular/common/http';
import { catchError, Observable, throwError } from 'rxjs';
import { Customer } from '../../../src/app/interfaces/get-customer-list-response';
import { environment } from '../../environments/environment';
import { HttpHeaderService } from './http-header-service';
import { GenericResponse } from '../interfaces/generic-response';

@Injectable({
  providedIn: 'root',
})
export class GetCustomerService {
  readonly APIURL =
    environment.CustomerManagementSystemAPI + '/api/Customer/get';

  constructor(
    private http: HttpClient,
    private httpHeaderService: HttpHeaderService,
  ) {}

  getCustomer(queryString: string): Observable<GenericResponse<Customer>> {
    const headers: HttpHeaders =
      this.httpHeaderService.getHeadersWithTokenSet();
    const params = new HttpParams().set('searchVariable', queryString);

    return this.http
      .get<GenericResponse<Customer>>(this.APIURL, {
        headers: headers,
        params: params,
      })
      .pipe(catchError(this.handleError));
  }
  private handleError(error: HttpErrorResponse): Observable<never> {
    console.error('Error fetching customers:', error);
    return throwError(() => new Error(error.message || 'Server error'));
  }
}
