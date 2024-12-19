import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Customer } from '../../../src/app/interfaces/get-customer-list-response';
import { environment } from '../../environments/environment';
import { HttpHeaderService } from './http-header-service';

@Injectable({
  providedIn: 'root',
})
export class GetCustomerService {
  readonly APIURL =
    environment.CustomerManagementSystemAPI + '/api/Customer/get';

  constructor(
    private httpHeaderService: HttpHeaderService,
    private http: HttpClient
  ) {}

  getCustomer(queryString: string): Observable<Customer> {
    const headers = this.httpHeaderService.getHeadersWithTokenSet();
    const params = new HttpParams().set('searchVariable', queryString);

    return this.http.get<Customer>(this.APIURL, {
      headers: headers,
      params: params,
    });
  }
}
