import { Component } from '@angular/core';
import { IcService } from 'src/model/ic/ic.service'
import { ConfirmPopup } from 'primeng/confirmpopup';
import { ConfirmationService } from 'primeng/api';

@Component({
    selector: 'app-embedded',
    imports: [ConfirmPopup],
    providers: [ConfirmationService],
    templateUrl: './embedded.component.html',
    styleUrl: './embedded.component.scss',
})
export class EmbeddedComponent {


    constructor(private srv: IcService, private confirmationService: ConfirmationService) {
    }

    confirmDelete(event: Event) {
        this.confirmationService.confirm({
            target: event.target as EventTarget,
            message: 'Tem certeza que deseja apagar todos os índices?',
            icon: 'pi pi-exclamation-triangle',
            acceptLabel: 'Sim',
            rejectLabel: 'Não',
            acceptButtonProps: {
                severity: 'danger',
            },
            accept: () => {
                // Adicione aqui a lógica para apagar
                this.apagaIndices();
            },
            reject: () => {
            }
        });
    }

    apagaIndices() {
        this.srv.EmbeddingZera().subscribe({});
    }

    refazIndices() {
        this.srv.EmbeddingRestante().subscribe({});
    }

}
