import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError } from 'rxjs';
import { segUsuarioService } from 'src/model/seg/usuario.service';





export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const srv = inject(segUsuarioService);
  const router = inject(Router);
  return next(req).pipe(
    catchError(error => {
      const eleErro = document.getElementById('dvErro');
      if (eleErro) {

        if (eleErro.style && eleErro.style.display) {
          eleErro.style.display = '';
        }
        eleErro.innerHTML = `<p><i class="pi pi-exclamation-triangle"></i>${error.error.mensagem}</p>`
        setTimeout(() => {
          eleErro.style.display = 'none';
        }, 4000)
      }

      throw error;
    })
  );
};


