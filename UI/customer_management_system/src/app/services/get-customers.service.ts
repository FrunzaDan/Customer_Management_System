import { Injectable } from '@angular/core';
import {
  HttpClient,
  HttpErrorResponse,
  HttpHeaders,
} from '@angular/common/http';
import { Observable, catchError, throwError } from 'rxjs';
import { environment } from '../../environments/environment';
import { HttpHeaderService } from './http-header-service';
import { GenericResponse } from '../interfaces/generic-response';
import { Customer } from '../interfaces/get-customer-list-response';

@Injectable({
  providedIn: 'root',
})
export class GetCustomersService {
  private readonly APIURL =
    environment.CustomerManagementSystemAPI + '/api/Customer/all';

  constructor(
    private http: HttpClient,
    private httpHeaderService: HttpHeaderService,
  ) {}

  refreshTable(): Observable<GenericResponse<Customer[]>> {
    const headers: HttpHeaders =
      this.httpHeaderService.getHeadersWithTokenSet();

    return this.http
      .get<GenericResponse<Customer[]>>(this.APIURL, { headers })
      .pipe(catchError(this.handleError));
  }

  private handleError(error: HttpErrorResponse): Observable<never> {
    console.error('Error fetching customers:', error);
    return throwError(() => new Error(error.message || 'Server error'));
  }
}
