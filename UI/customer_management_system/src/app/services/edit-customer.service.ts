import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class EditCustomerService {

  readonly APIURL = "https://localhost:7145/Customer/GetCustomers";

  constructor(private http: HttpClient) {
  }

  // editCustomer():Observable<GetCustomerListResponse> {
  //   const headers = new HttpHeaders()
  //     .set('content-type', 'application/json')
  //     .set('Access-Control-Allow-Origin', '*')
  //     .set('Authorization', 'Bearer ' + sessionStorage.getItem('accessToken'));

  //   return this.http.get<GetCustomerListResponse>(this.APIURL, { 'headers': headers })
  // }
}
