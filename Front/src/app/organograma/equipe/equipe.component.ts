import { Component, effect, input, signal } from '@angular/core';
import { segOrganograma } from 'src/model/seg/organograma';
import { segEquipe } from 'src/model/seg/equipe';
import { EquipeService } from 'src/model/seg/equipe.service';
import { TableModule } from 'primeng/table';
import { ConfirmPopupModule } from 'primeng/confirmpopup';
import { ConfirmationService } from 'primeng/api';
import { UsuarioAutoCompleteComponent } from 'src/app/usuario/auto-complete/auto-complete.component'



@Component({
    selector: 'app-equipe',
    imports: [TableModule, ConfirmPopupModule, UsuarioAutoCompleteComponent],
    templateUrl: './equipe.component.html',
    styleUrl: './equipe.component.scss',
    providers: [ConfirmationService]
})
export class EquipeComponent {
    org = input<segOrganograma | undefined>();
    lista = signal<segEquipe[]>([]);

    constructor(private srv: EquipeService, private confirmationService: ConfirmationService) {
        effect(() => {
            if (this.org()) {
                this.atualiza();
            } else {
            }
        });

    }

    private atualiza() {
        this.srv.UsuarioPorOrganograma(this.org()!.id).subscribe({
            next: (dados: segEquipe[]) => { this.lista.set(dados); }
        });
    }

    exclui(item: segEquipe, event: any) {
        this.confirmationService.confirm({
            target: event.currentTarget as EventTarget,
            message: 'Confirma a exclusão deste usuário da equipe?',
            icon: 'pi pi-exclamation-triangle',
            rejectButtonProps: {
                label: 'Cancela',
                severity: 'secondary',
                outlined: true
            },
            acceptButtonProps: {
                label: 'Exclui',
                severity: 'danger'
            },
            accept: () => {
                this.srv.Exclui(item.id).subscribe({
                    next: () => { this.atualiza(); }
                });
            }
        });
    }

}
