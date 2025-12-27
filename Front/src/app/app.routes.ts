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
import { EditMeComponent } from 'src/app/edit-me/edit-me.component';


export const routes: Routes = [
    { path: 'home', component: PrincipalComponent, canActivate: [autenticadoGuard], title: 'CMDB' },
    { path: 'home/:id', component: PrincipalComponent, canActivate: [autenticadoGuard], title: 'CMDB' },
    { path: 'organograma', component: OrganogramaPrincipalComponent, canActivate: [adminGuard], title: 'Organogramas' },
    { path: 'configuracao', component: ConfiguracaoComponent, canActivate: [adminGuard], title: 'Configuração' },
    { path: 'usuario/cadastro', component: UsuarioCadastroComponent, canActivate: [adminGuard], title: 'Cadastro de Usuário' },
    { path: 'tipo', component: TipoComponent, canActivate: [adminGuard], title: 'Tipos' },
    { path: 'publico', component: PublicoComponent, title: 'CMDB - Acesso Público' },
    { path: 'recuperacao-senha', component: RecuperacaoSenhaComponent, title: 'Recuperação de Senha' },
    { path: 'embedded', component: EmbeddedComponent, canActivate: [adminGuard], title: 'Embedded' },
    { path: 'edit-me', component: EditMeComponent, canActivate: [autenticadoGuard], title: 'Editar Perfil' },
    { path: '', redirectTo: '/home', pathMatch: 'full' },
    { path: '**', redirectTo: '/home' },
];
