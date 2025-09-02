import { Routes } from '@angular/router';
import { PrincipalComponent } from 'src/app/principal/principal.component'
import { PublicoComponent } from 'src/app/publico/publico.component'
import { autenticadoGuard } from 'src/guard/autenticado';
import { OrganogramaPrincipalComponent } from 'src/app/organograma/principal/principal.component'
import { ConfiguracaoComponent } from 'src/app/configuracao/configuracao.component'

export const routes: Routes = [
    { path: 'home', component: PrincipalComponent, canActivate: [autenticadoGuard] },
    { path: 'organograma', component: OrganogramaPrincipalComponent, canActivate: [autenticadoGuard] },
    { path: 'configuracao', component: ConfiguracaoComponent, canActivate: [autenticadoGuard] },
    { path: 'publico', component: PublicoComponent },
    { path: '', redirectTo: '/home', pathMatch: 'full' },
    { path: '**', redirectTo: '/home' },
];
