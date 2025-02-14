import { Injectable } from '@angular/core';
import {
  HttpClient,
  HttpErrorResponse,
  HttpHeaders,
  HttpParams,
} from '@angular/common/http';
import { catchError, Observable, throwError } from 'rxjs';
import { environment } from '../../environments/environment';
import { HttpHeaderService } from './http-header-service';
import { GenericResponse } from '../interfaces/generic-response';

@Injectable({
  providedIn: 'root',
})
export class DeleteCustomerService {
  readonly APIURL =
    environment.CustomerManagementSystemAPI + '/api/Customer/delete';

  constructor(
    private http: HttpClient,
    private httpHeaderService: HttpHeaderService,
  ) {}

  deleteCustomer(queryString: string): Observable<GenericResponse<object>> {
    const headers: HttpHeaders =
      this.httpHeaderService.getHeadersWithTokenSet();
    const params = new HttpParams().set('customerGUID', queryString);

    return this.http
      .delete<GenericResponse<object>>(this.APIURL, {
        headers: headers,
        params: params,
      })
      .pipe(catchError(this.handleError));
  }

  private handleError(error: HttpErrorResponse): Observable<never> {
    console.error('Error:', error);
    return throwError(() => new Error(error.message || 'Server error'));
  }
}
