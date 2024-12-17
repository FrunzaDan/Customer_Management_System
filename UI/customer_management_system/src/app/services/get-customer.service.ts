import { Injectable } from '@angular/core';
import {
  HttpClient,
  HttpParams,
  HttpParamsOptions,
} from '@angular/common/http';
import { HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Customer } from 'src/app/interfaces/get-customer-list-response';
import { environment } from '../../environments/environment';
import { SessionStorageService } from './session-storage.service';

@Injectable({
  providedIn: 'root',
})
export class GetCustomerService {
  readonly APIURL =
    environment.CustomerManagementSystemAPI + '/api/Customer/get';

  constructor(
    private sessionStorageService: SessionStorageService,
    private http: HttpClient
  ) {}

  getCustomer(queryString: string): Observable<Customer> {
    const headers = new HttpHeaders()
      .set('content-type', 'application/json')
      .set('Access-Control-Allow-Origin', '*')
      .set(
        'Authorization',
        'Bearer ' + this.sessionStorageService.getSessionAccessToken()
      );

    const params = new HttpParams().set('searchVariable', queryString);

    return this.http.get<Customer>(this.APIURL, {
      headers: headers,
      params: params,
    });
  }
}
