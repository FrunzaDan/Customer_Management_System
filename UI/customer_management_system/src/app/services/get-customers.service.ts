import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, catchError, map, throwError, tap } from 'rxjs';
import { environment } from '../../environments/environment';
import { HttpHeaderService } from './http-header-service';
import { GenericResponse } from '../interfaces/generic-response';
import { Customer } from '../interfaces/get-customer-list-response';

@Injectable({
  providedIn: 'root',
})
export class GetCustomersService {
  readonly APIURL =
    environment.CustomerManagementSystemAPI + '/api/Customer/all';

  constructor(
    private http: HttpClient,
    private httpHeaderService: HttpHeaderService,
  ) {}

  refreshTable(): Observable<GenericResponse<Customer[]>> {
    const headers = this.httpHeaderService.getHeadersWithTokenSet();

    return this.http
      .get<GenericResponse<Customer[]>>(this.APIURL, { headers })
      .pipe(
        catchError((error: HttpErrorResponse) => {
          console.error('Error fetching customers:', error);
          return throwError(() => error);
        }),
      );
  }
}
