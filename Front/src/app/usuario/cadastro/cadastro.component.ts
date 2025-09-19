import { Component, OnInit, signal } from '@angular/core';
import { TableModule } from 'primeng/table';
import { segUsuarioService } from 'src/model/seg/usuario.service'
import { segUsuario } from 'src/model/seg/usuario'

@Component({
    selector: 'app-cadastro',
    imports: [TableModule],
    templateUrl: './cadastro.component.html',
    styleUrl: './cadastro.component.scss'
})
export class UsuarioCadastroComponent implements OnInit {

    listaUsuarios = signal<any[]>([]);
    constructor(private usuarioService: segUsuarioService) { }

    ngOnInit(): void {
        this.usuarioService.lista().subscribe(usuarios => {
            this.listaUsuarios.set(usuarios);
        });
    }

}
