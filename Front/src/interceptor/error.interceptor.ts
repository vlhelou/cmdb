import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError } from 'rxjs';
import { segUsuarioService } from 'src/model/seg/usuario.service';





export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const srv=inject(segUsuarioService);
  const router = inject(Router);
  return next(req).pipe(
    catchError(error => {
      if (error.status === 401) {
        console.log('401 - Unauthorized');
        srv.Logout();
        router.navigate(['/publico']);
      } else if (error.status === 500) {
        // Exibir uma mensagem de erro ao usuário
      }
      throw error;
    })
  );
};


