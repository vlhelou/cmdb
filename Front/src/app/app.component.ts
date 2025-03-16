import { Component, OnInit } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { ToolbarModule } from 'primeng/toolbar';
import { SegUsuarioService } from 'src/model/seg/usuario.service';
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

    constructor(private srv: SegUsuarioService) {
            this.srv.currentUser.subscribe({
                next: (data) => {
                    console.log(data);
                }
            });
    }

    ngOnInit(): void {
        this.items = [
            {
                label: 'Home',
                icon: 'pi pi-home'
            },
            {
                label: 'Organograma',
                icon: 'pi pi-sitemap'
            },
            {
                label: 'Contato',
                icon: 'pi pi-envelope'
            }
        ]
    }
}
