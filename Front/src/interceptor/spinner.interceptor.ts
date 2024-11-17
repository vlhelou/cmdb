import { Injectable } from '@angular/core';
import * as util from 'src/app/util';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor } from '@angular/common/http';
import { Observable, finalize } from 'rxjs';

@Injectable()
export class SpinnerInterceptor implements HttpInterceptor {
    private util = util;
    private activeRequest = 0;
    constructor() { }

    intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
        this.activeRequest++;
        if (this.activeRequest > 0) {
            this.util.spinnerOn();
        }
        return next.handle(request).pipe(finalize(() => {
            this.activeRequest--;
            if (this.activeRequest === 0) {
                this.util.spinnerOff();
            }
        }));
    }
}
