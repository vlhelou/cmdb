import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor,
} from '@angular/common/http';
import { Observable } from 'rxjs';
import { SegurancaService } from 'src/model/seguranca.service';

// import { UsuarioService } from 'src/app/model/usuario/usuario.service';

// export const jwtInterceptor: HttpInterceptorFn = (req, next) => {
//     const usuarioService = inject(UsuarioService);
//     const usuario = usuarioService.autenticadoValue;
//     if (usuario && usuario.token) {
//         req = req.clone({
//             setHeaders: {
//                 Authorization: `Bearer ${usuario.token}`
//             }
//         });
//     }
//     return next(req);
// };

@Injectable()
export class jwtInterceptor implements HttpInterceptor {
  constructor(private srv: SegurancaService) {}

  intercept(
    request: HttpRequest<any>,
    next: HttpHandler
  ): Observable<HttpEvent<any>> {
    // add authorization header with jwt token if available
    const currentUser = this.srv.currentUserValue;
    if (currentUser && currentUser.token) {
        request = request.clone({
            setHeaders: {
                Authorization: `Bearer ${currentUser.token}`
            }
        });
    }
    return next.handle(request);
  }
}
