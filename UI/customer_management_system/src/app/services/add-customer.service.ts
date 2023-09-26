import { Injectable } from '@angular/core';
import { HttpClient, HttpParams, HttpParamsOptions } from '@angular/common/http';
import { HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Customer } from 'src/app/interfaces/get-customer-list-response';
import { GenericResponse } from 'src/app/interfaces/generic-response';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AddCustomerService {

  readonly APIURL = environment.CustomerManagementSystemAPI + "/Customer/RegisterCustomer";
  httpOptions = {
    headers: new HttpHeaders({
      'Content-Type': 'application/json',
      'Access-Control-Allow-Origin': '*',
      'Authorization': 'Bearer ' + sessionStorage.getItem('accessToken')
    }),
  };

  constructor(private http: HttpClient) { }

  addCustomer(customer: Customer): Observable<GenericResponse> {
    console.log("addCustomerService entered:");
    console.log(customer);
    return this.http.post<GenericResponse>(
      this.APIURL,
      customer,
      this.httpOptions
    );
  };
}
