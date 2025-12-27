import { Component, OnInit, signal } from '@angular/core';
import { segUsuario } from 'src/model/seg/usuario'
import { FormsModule } from '@angular/forms';
import { segUsuarioService } from 'src/model/seg/usuario.service'
import { TableModule } from 'primeng/table';
import { segEquipe } from 'src/model/seg/equipe';
import { EquipeService } from 'src/model/seg/equipe.service'
import { ConfirmPopupModule } from 'primeng/confirmpopup';
import { ConfirmationService } from 'primeng/api';
import { IcSegredoService } from 'src/model/ic/segredo.service'
import { DialogModule } from 'primeng/dialog';
import { RouterLink } from "@angular/router";


@Component({
    selector: 'app-edit-me',
    imports: [FormsModule, TableModule, ConfirmPopupModule, DialogModule, RouterLink],
    templateUrl: './edit-me.component.html',
    styleUrl: './edit-me.component.scss',
    providers: [ConfirmationService]
})
export class EditMeComponent implements OnInit {

    eu = signal<segUsuario | null>(null);
    email: string = '';
    equipes = signal<segEquipe[]>([]);
    segredos = signal<any[]>([]);
    conteudo = signal<string>('');
    mostraSegredo = false;

    constructor(
        private srv: segUsuarioService,
        private confirmationService: ConfirmationService,
        private equipe: EquipeService,
        private segredo: IcSegredoService
    ) { }

    ngOnInit(): void {
        this.atualiza();
    }

    private atualiza() {
        this.srv.Eu().subscribe({
            next: (r) => {
                this.eu.set(r);
                this.email = r.email;
                this.equipes.set(r.locacoes || []);
            }
        });

        this.segredo.MeusSegredos({}).subscribe({
            next: (r) => {
                this.segredos.set(r);
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

    mostraConteudo(id: number) {
        this.segredo.Visualiza(id).subscribe({
            next: (data) => {
                this.mostraSegredo = true;
                this.conteudo.set(data.conteudo || '');
                // Manipule os dados recebidos conforme necessário
            }
        });

    }


}
