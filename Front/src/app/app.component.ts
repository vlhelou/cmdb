import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { ToolbarModule } from 'primeng/toolbar';
import { SegUsuarioService } from 'src/model/seg/usuario.service';

@Component({
    selector: 'app-root',
    standalone: true,
    imports: [RouterOutlet, ToolbarModule],
    templateUrl: './app.component.html',
    styleUrl: './app.component.scss'
})
export class AppComponent {



    constructor(private srv: SegUsuarioService) {
        this.srv.currentUser.subscribe({
            next: (data) => {
                console.log(data);
            }
        });
    }
}
