import { inject } from '@angular/core';
import { CanActivateFn } from '@angular/router';
import { Router } from '@angular/router';
// import { SegurancaService } from 'src/model/seguranca.service';

export const autenticadoGuard: CanActivateFn = (route, state) => {
    // let srv: SegurancaService;
    // srv = inject(SegurancaService);

    // const strcurrentUser = srv.currentUserValue;
    const strcurrentUser = {};
    if (!strcurrentUser) {
        inject(Router).navigate(['/login'],{
            queryParams: { returnUrl: state.url },
        });
        return false;
    } else {
        return true;
    }
};
