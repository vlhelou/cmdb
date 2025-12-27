import { Component, OnInit, signal } from '@angular/core';
import { segUsuario } from 'src/model/seg/usuario'
import { FormsModule } from '@angular/forms';
import { segUsuarioService } from 'src/model/seg/usuario.service'
import { TableModule } from 'primeng/table';
import { segEquipe } from 'src/model/seg/equipe';
import { EquipeService } from 'src/model/seg/equipe.service'
import { ConfirmPopupModule } from 'primeng/confirmpopup';
import { ConfirmationService } from 'primeng/api';

@Component({
    selector: 'app-edit-me',
    imports: [FormsModule, TableModule, ConfirmPopupModule],
    templateUrl: './edit-me.component.html',
    styleUrl: './edit-me.component.scss',
    providers: [ConfirmationService]
})
export class EditMeComponent implements OnInit {

    eu = signal<segUsuario | null>(null);
    email: string = '';
    equipes = signal<segEquipe[]>([]);

    constructor(
        private srv: segUsuarioService,
        private confirmationService: ConfirmationService,
        private equipe: EquipeService
    ) { }

    ngOnInit(): void {
        this.atualiza();
    }
    
    private atualiza(){
        this.srv.Eu().subscribe({
            next: (r) => {
                console.log(r);
                this.eu.set(r);
                this.email = r.email;
                this.equipes.set(r.locacoes || []);
            }
        });

    }

    alteraEmail() {
        this.srv.AlteraEmail(this.email).subscribe({
            next: (res) => {
                this.email = res.email;
            }
        });
    }


    exclusaoOrganograma(event: Event, id: number) {
        this.confirmationService.confirm({
            target: event.currentTarget as EventTarget,
            message: 'Confirma a exclusão?',
            icon: 'pi pi-exclamation-triangle',
            rejectLabel: 'Cancelar',
            acceptLabel: 'Excluir',
            rejectButtonProps: {
                label: 'Cancel',
                outlined: true
            },
            acceptButtonProps: {
                severity: 'danger',
            },
            accept: () => {
                this.equipe.ExcluiMeuOrganograma(id).subscribe({
                    next: () => {
                        this.atualiza();
                    }
                });
            },
            reject: () => {
            }
        });
    }
}
