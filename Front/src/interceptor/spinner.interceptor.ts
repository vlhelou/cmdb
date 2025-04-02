import { HttpInterceptorFn } from '@angular/common/http';
import * as util from 'src/app/util';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor } from '@angular/common/http';
import { Observable, finalize } from 'rxjs';

export const spinnerInterceptor: HttpInterceptorFn = (req, next) => {
    util.spinnerOn();

    return next(req).pipe(finalize(() => {
        util.spinnerOff();
    }));


};
