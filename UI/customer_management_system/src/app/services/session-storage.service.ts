import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class SessionStorageService {
  private readonly accessTokenKey = 'accessToken';

  getSessionAccessToken(): string {
    if (typeof window !== 'undefined') {
      try {
        const accessTokenString = sessionStorage.getItem(this.accessTokenKey);

        return accessTokenString ?? 'ERROR-NO-SESSION-TOKEN';
      } catch (parseError: unknown) {
        console.error(
          'Error parsing products from session storage:',
          parseError,
        );
        return 'ERROR-NO-SESSION-TOKEN';
      }
    } else {
      return 'ERROR-NON-BROWSER-ENVIRONMENT';
    }
  }

  setSessionAccessToken(sessionStorageAccessToken: string): void {
    if (typeof window !== 'undefined') {
      sessionStorage.setItem('accessToken', sessionStorageAccessToken);
    }
  }

  removeSessionStorage(): void {
    if (typeof window !== 'undefined') {
      sessionStorage.clear();
    }
  }
}
