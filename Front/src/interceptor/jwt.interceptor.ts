import { HttpEvent, HttpHandlerFn, HttpInterceptorFn, HttpRequest } from '@angular/common/http';
import { Inject } from '@angular/core';
import { Observable } from 'rxjs';
import { segUsuarioService } from 'src/model/seg/usuario.service';


export function jwtInterceptor(req: HttpRequest<unknown>, next: HttpHandlerFn): Observable<HttpEvent<unknown>> {
  const usuarioService = Inject(segUsuarioService);
  const usuario = usuarioService.usuarioAtual();
  console.log('usuario', usuario);
  console.log(req.url);
  return next(req);
}

// export function jwtInterceptor(req: any, next: any) {
//   const usuarioService = Inject(segUsuarioService);
//   const usuario = usuarioService.usuarioAtual();
//   console.log('usuario', usuario);
//   if (usuario && usuario.token) {
//     req = req.clone({
//       setHeaders: {
//         Authorization: `Bearer ${usuario.token}`
//       }
//     });
//   }
//   return next(req);
// }

// export const jwtInterceptor: HttpInterceptorFn = (req, next) => {
//   const usuarioService = Inject(segUsuarioService);
//   const usuario = usuarioService.usuarioAtual();
//   console.log('usuario', usuario);
//   if (usuario && usuario.token) {
//     req = req.clone({
//       setHeaders: {
//         Authorization: `Bearer ${usuario.token}`
//       }
//     });
//   }
//   return next(req);
// };
