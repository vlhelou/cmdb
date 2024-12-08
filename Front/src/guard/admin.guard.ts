import { CanActivateFn } from '@angular/router';
import { Router } from '@angular/router';
import { SegUsuarioService } from 'src/model/seg/usuario.service';
import { inject } from '@angular/core';

export const adminGuard: CanActivateFn = (route, state) => {
  let srv: SegUsuarioService;
  srv = inject(SegUsuarioService);

  const currentUser = srv.currentUserValue;
  const teste = srv.usuarioAtual();

  if (currentUser && currentUser.administrador) {
    return true;
  }

  inject(Router).navigate(['/home'], {
    queryParams: { returnUrl: state.url },
  });
  return false;

};
