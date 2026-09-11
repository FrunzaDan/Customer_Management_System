import {
  provideHttpClient,
  withFetch,
  withInterceptors,
  withInterceptorsFromDi,
} from '@angular/common/http';
import { ApplicationConfig, provideZoneChangeDetection } from '@angular/core';
import {
  provideClientHydration,
  withEventReplay,
  withNoIncrementalHydration
} from '@angular/platform-browser';
import { provideAnimations } from '@angular/platform-browser/animations';
import {
  provideRouter,
  withComponentInputBinding,
  withInMemoryScrolling,
  withViewTransitions,
} from '@angular/router';
import { routes } from './app.routes';
import { authErrorInterceptor } from './services/auth-error.interceptor';

export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(
      routes,
      withComponentInputBinding(),
      withInMemoryScrolling({
        scrollPositionRestoration: 'top',
        anchorScrolling: 'enabled',
      }),
      withViewTransitions(),
    ),
    provideClientHydration(withEventReplay(), withNoIncrementalHydration()),
    provideHttpClient(
      withFetch(),
      withInterceptorsFromDi(),
      withInterceptors([authErrorInterceptor]),
    ),
    provideAnimations(),
  ],
};
