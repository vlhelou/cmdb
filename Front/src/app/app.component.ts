import { Component, OnInit } from '@angular/core';
import { RouterOutlet, Router } from '@angular/router';
import { ToolbarModule } from 'primeng/toolbar';
import { segUsuarioService } from 'src/model/seg/usuario.service';
import { MenubarModule } from 'primeng/menubar';
import { MenuItem } from 'primeng/api';

@Component({
    selector: 'app-root',
    standalone: true,
    imports: [RouterOutlet, ToolbarModule, MenubarModule],
    templateUrl: './app.component.html',
    styleUrl: './app.component.scss'
})
export class AppComponent implements OnInit {

    items: MenuItem[] | undefined;

    constructor(private srv: segUsuarioService, private router: Router) {
            this.srv.currentUser.subscribe({
                next: (data) => {
                }
            });
    }

    ngOnInit(): void {
        this.items = [
            {
                label: 'Home',
                icon: 'pi pi-home',
                routerLink: '/home'
            },
            {
                label: 'Organograma',
                icon: 'pi pi-sitemap',
                routerLink: '/organograma'
            },
            {
                label: 'Usuário',
                icon: 'pi pi-user',
                routerLink: '/usuario/cadastro'
            },
            {
                label: 'Configuração',
                icon: 'pi pi-cog',
                routerLink: '/configuracao'
            },
            {
                label: 'Tipo',
                icon: 'pi pi-list-check',
                routerLink: '/tipo'
            },
            {
                label: 'Contato',
                icon: 'pi pi-envelope'
            }
        ]
    }

    sair() {
        this.router.navigate(['/publico']);
        this.srv.Logout();
    }
}
