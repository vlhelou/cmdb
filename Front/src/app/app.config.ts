import { ApplicationConfig, provideZoneChangeDetection, LOCALE_ID, importProvidersFrom } from '@angular/core';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { provideRouter } from '@angular/router';
import { registerLocaleData } from '@angular/common';
import localePt from '@angular/common/locales/pt';
import { routes } from './app.routes';
import { providePrimeNG } from 'primeng/config';
import lara from '@primeng/themes/lara'
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { spinnerInterceptor } from 'src/interceptor/spinner.interceptor';
import { ErrorInterceptor } from 'src/interceptor/error.interceptor';
import { jwtInterceptor } from 'src/interceptor/jwt.interceptor';

registerLocaleData(localePt);

export const appConfig: ApplicationConfig = {
    providers: [
        provideZoneChangeDetection({ eventCoalescing: true }),
        provideRouter(routes),
        provideHttpClient(withInterceptors([jwtInterceptor, spinnerInterceptor])),
        importProvidersFrom([BrowserAnimationsModule]),
        { provide: LOCALE_ID, useValue: 'pt-BR' },
        providePrimeNG({
            theme: {
                preset: lara
            }
        })
    ],

};
