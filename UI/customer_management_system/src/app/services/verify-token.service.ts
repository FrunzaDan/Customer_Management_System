import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Router } from '@angular/router';
import { GenericResponse } from '../../../src/app/interfaces/generic-response';
import { Subject, Observable } from 'rxjs';
import { environment } from '../../../src/environments/environment';
import { SessionStorageService } from './session-storage.service';

@Injectable({
  providedIn: 'root',
})
export class VerifyTokenService {
  readonly APIURL =
    environment.CustomerManagementSystemAPI +
    '/api/Authentication/verify-token';
  httpOptions = {
    headers: new HttpHeaders({
      'Content-Type': 'application/json',
      'Access-Control-Allow-Origin': '*',
    }),
  };

  constructor(
    private http: HttpClient,
    private router: Router,
    private sessionStorageService: SessionStorageService
  ) {}

  isTokenValid(): Observable<boolean> {
    const result = new Subject<boolean>();

    this.verifyTokenViaAPI().subscribe({
      next: (response) => {
        if (response.status == '200') {
          result.next(true);
        } else if (response.status == '403') {
          result.next(false);
          result.complete();
        } else {
          result.next(false);
          result.complete();
        }
      },
      error: (error) => {
        if (error.error.responseCode == 403) {
          console.log('Forbidden Access!');
        }

        result.next(false);
        result.complete();
      },
    });
    return result.asObservable();
  }

  verifyTokenViaAPI(): Observable<GenericResponse> {
    let accessTokenFromSession =
      this.sessionStorageService.getSessionAccessToken();

    const headers = new HttpHeaders()
      .set('content-type', 'application/json')
      .set('Access-Control-Allow-Origin', '*');

    const params = new HttpParams().set('accessToken', accessTokenFromSession);

    return this.http.get<GenericResponse>(this.APIURL, {
      headers: headers,
      params: params,
    });
  }
}
