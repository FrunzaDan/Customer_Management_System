import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Customer } from '../../../src/app/interfaces/get-customer-list-response';
import { GenericResponse } from '../../../src/app/interfaces/generic-response';
import { environment } from '../../environments/environment';
import { HttpHeaderService } from './http-header-service';

@Injectable({
  providedIn: 'root',
})
export class EditCustomerService {
  constructor(
    private httpHeaderService: HttpHeaderService,
    private http: HttpClient
  ) {}

  readonly APIURL =
    environment.CustomerManagementSystemAPI + '/api/Customer/edit';

  editCustomer(customer: Customer): Observable<GenericResponse<object>> {
    const headers = this.httpHeaderService.getHeadersWithTokenSet();
    return this.http.patch<GenericResponse<object>>(this.APIURL, customer, {
      headers: headers,
    });
  }
}
