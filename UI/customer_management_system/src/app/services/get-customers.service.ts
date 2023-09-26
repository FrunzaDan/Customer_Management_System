import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpErrorResponse } from '@angular/common/http';
import { Observable, catchError, throwError } from 'rxjs';
import { GetCustomerListResponse } from 'src/app/interfaces/get-customer-list-response';
import { environment } from '../../environments/environment';


@Injectable({
  providedIn: 'root'
})
export class GetCustomersService {
  readonly APIURL = environment.CustomerManagementSystemAPI + "/Customer/GetCustomers";

  constructor(private http: HttpClient) {
  }

  refreshTable():Observable<GetCustomerListResponse> {
    const headers = new HttpHeaders()
      .set('content-type', 'application/json')
      .set('Access-Control-Allow-Origin', '*')
      .set('Authorization', 'Bearer ' + sessionStorage.getItem('accessToken'));

    return this.http.get<GetCustomerListResponse>(this.APIURL, { 'headers': headers }).pipe(
      catchError(this.handleError));
    
  }

  private handleError(error: HttpErrorResponse){
    if(error.status===0){
      console.error('An error occured: ', error.error)
    } else {
      console.error(`Backend returned code ${error.status}, body was: `, error.error)
    }
    return throwError(() => new Error('Something bad happened, please try again later.'));
  }
}
