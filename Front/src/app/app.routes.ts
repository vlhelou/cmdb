import { Routes } from '@angular/router';
import { PrincipalComponent } from 'src/app/principal/principal.component'
import { PublicoComponent } from 'src/app/publico/publico.component'
import { autenticadoGuard } from 'src/guard/autenticado';
import { adminGuard } from 'src/guard/admin.guard';
import { OrganogramaPrincipalComponent } from 'src/app/organograma/principal/principal.component'
import { ConfiguracaoComponent } from 'src/app/configuracao/configuracao.component'
import { UsuarioCadastroComponent } from 'src/app/usuario/cadastro/cadastro.component'
import { RecuperacaoSenhaComponent } from 'src/app/recuperacao-senha/recuperacao-senha.component'
import { TipoComponent } from 'src/app/tipo/tipo.component';
import { EmbeddedComponent } from 'src/app/embedded/embedded.component';


export const routes: Routes = [
    { path: 'home', component: PrincipalComponent, canActivate: [autenticadoGuard] },
    { path: 'organograma', component: OrganogramaPrincipalComponent, canActivate: [adminGuard] },
    { path: 'configuracao', component: ConfiguracaoComponent, canActivate: [adminGuard] },
    { path: 'usuario/cadastro', component: UsuarioCadastroComponent, canActivate: [adminGuard] },
    { path: 'tipo', component: TipoComponent, canActivate: [adminGuard] },
    { path: 'publico', component: PublicoComponent },
    { path: 'recuperacao-senha', component: RecuperacaoSenhaComponent },
    { path: 'embedded', component: EmbeddedComponent, canActivate: [adminGuard] },
    { path: '', redirectTo: '/home', pathMatch: 'full' },
    { path: '**', redirectTo: '/home' },
];
