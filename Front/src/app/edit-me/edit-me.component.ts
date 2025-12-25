import { Component, OnInit, signal } from '@angular/core';
import { segUsuario } from 'src/model/seg/usuario'
import { FormsModule } from '@angular/forms';
import { segUsuarioService } from 'src/model/seg/usuario.service'
import { TableModule } from 'primeng/table';
import { segEquipe } from 'src/model/seg/equipe';

@Component({
    selector: 'app-edit-me',
    imports: [FormsModule, TableModule],
    templateUrl: './edit-me.component.html',
    styleUrl: './edit-me.component.scss',
})
export class EditMeComponent implements OnInit {

    eu = signal<segUsuario | null>(null);
    email: string = '';
    equipes = signal<segEquipe[]>([]);

    constructor(private srv: segUsuarioService) { }

    ngOnInit(): void {
        this.srv.Eu().subscribe({
            next: (r) => {
                console.log(r);
                this.eu.set(r);
                this.email = r.email;
                this.equipes.set(r.locacoes || []);
            }
        });
    }


    alteraEmail(){
        this.srv.AlteraEmail(this.email).subscribe({
            next:(res)=>{
                this.email = res.email;
            }
        });
    }
}
