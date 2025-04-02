import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { segUsuarioService } from 'src/model/seg/usuario.service';

export const jwtInterceptor: HttpInterceptorFn = (req, next) => {
  const srv=inject(segUsuarioService);
  const usuarioLogado = srv.usuarioAtual();
  const token = usuarioLogado?.token;
  if (token) {
    req = req.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`,
      },
    });
  }

  return next(req);
};
