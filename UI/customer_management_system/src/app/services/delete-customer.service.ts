import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, tap } from 'rxjs';
import { environment } from '../../environments/environment';
import { GenericResponse } from '../interfaces/generic-response';
import { GetCustomerService } from './get-customer.service';
import { HttpHeaderService } from './http-header-service';

@Injectable({
  providedIn: 'root',
})
export class DeleteCustomerService {
  readonly APIURL =
    environment.CustomerManagementSystemAPI + '/api/Customer/delete';

  constructor(
    private http: HttpClient,
    private httpHeaderService: HttpHeaderService,
    private getCustomerService: GetCustomerService,
  ) {}

  deleteCustomer(customerGUID: string): Observable<GenericResponse<object>> {
    const headers: HttpHeaders =
      this.httpHeaderService.getHeadersWithTokenSet();
    const params = new HttpParams().set('customerGUID', customerGUID);

    return this.http
      .delete<GenericResponse<object>>(this.APIURL, { headers, params })
      .pipe(
        tap(() => this.getCustomerService.removeCustomerLocally(customerGUID)),
      );
  }
}
