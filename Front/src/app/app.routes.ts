import { Routes } from '@angular/router';
import { PrincipalComponent } from 'src/app/principal/principal.component'
import { PublicoComponent } from 'src/app/publico/publico.component'
import { autenticadoGuard } from 'src/guard/autenticado';

export const routes: Routes = [
    { path: 'home', component: PrincipalComponent, canActivate: [autenticadoGuard] },
    { path: 'publico', component: PublicoComponent },
    { path: '', redirectTo: '/home', pathMatch: 'full' },
    { path: '**', redirectTo: '/home' },
];
