import { ApplicationConfig, provideZoneChangeDetection, LOCALE_ID, importProvidersFrom } from '@angular/core';
import { HTTP_INTERCEPTORS, withInterceptorsFromDi, provideHttpClient } from '@angular/common/http';
import { provideRouter } from '@angular/router';
import { registerLocaleData } from '@angular/common';
import localePt from '@angular/common/locales/pt';
import { routes } from './app.routes';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { SpinnerInterceptor } from 'src/interceptor/spinner.interceptor';

registerLocaleData(localePt);

export const appConfig: ApplicationConfig = {
    providers: [
        provideZoneChangeDetection({ eventCoalescing: true }),
        provideRouter(routes),
        provideHttpClient(withInterceptorsFromDi()),
        importProvidersFrom([BrowserAnimationsModule]),
        { provide: LOCALE_ID, useValue: 'pt-BR' },
        { provide: HTTP_INTERCEPTORS, useClass: SpinnerInterceptor, multi: true },
    ],

};
