import { ElementRef, Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor } from '@angular/common/http';
import {
    Observable,
    throwError
} from 'rxjs';
import { catchError } from 'rxjs/operators';


@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
    constructor( private router: Router) { }

    intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {

        return next.handle(request).pipe(catchError(err => {
            if (err.status === 401 || err.status === 403) {
                this.router.navigate(['/home', { queryParams: { acesso: 'negado' } }]);
                // auto logout if 401 response returned from api
                // this.authService.Logout();
                // location.reload();
            }
            else if ((err.status >= 500 && err.status <= 599) || err.status === 400) {
                const eleErro = document.getElementById('dvErro')
                if (eleErro){

                    if (eleErro.style && eleErro.style.display ){
                        eleErro.style.display = '';
                    }
                    eleErro.innerHTML = `<p><i class="pi pi-exclamation-triangle"></i>${err.error.mensagem}</p>`
                    setTimeout(() => {
                        eleErro.style.display = 'none';
                    }, 4000)
                }
            }

            const error = err.error.message || err.statusText;
            throw error;
        }));
    }
}
