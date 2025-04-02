import { inject } from '@angular/core';
import { CanActivateFn } from '@angular/router';
import { Router } from '@angular/router';
import { segUsuarioService } from 'src/model/seg/usuario.service';

export const autenticadoGuard: CanActivateFn = (route, state) => {
    let srv: segUsuarioService;
    srv = inject(segUsuarioService);

    const currentUser = srv.currentUserValue;
    const teste = srv.usuarioAtual();

    if (currentUser) {
        return true;
    }

    inject(Router).navigate(['/publico'], {
        queryParams: { returnUrl: state.url },
    });
    return false;

};
