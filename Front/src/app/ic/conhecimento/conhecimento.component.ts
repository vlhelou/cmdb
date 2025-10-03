import { Component, effect, input, signal, OnInit } from '@angular/core';
import { DatePipe } from '@angular/common';
import { icIc } from 'src/model/ic/ic';
import { IcConhecimento } from 'src/model/ic/conhecimento';
import { IcConhecimentoService } from 'src/model/ic/conhecimento.service';
import { FormControl, FormGroup, Validators, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { ConfirmationService } from 'primeng/api';
import { TableModule } from 'primeng/table';

@Component({
    selector: 'app-conhecimento',
    imports: [
        FormsModule,
        ReactiveFormsModule,
        ConfirmDialogModule,
        TableModule,
        DatePipe
    ],
    templateUrl: './conhecimento.component.html',
    styleUrl: './conhecimento.component.scss',
    providers: [ConfirmationService]
})
export class ConhecimentoComponent {
    ic = input<icIc | undefined>();
    lista = signal<IcConhecimento[]>([]);

    form = new FormGroup({
        id: new FormControl<number>(0),
        idIc: new FormControl<number>(0),
        problema: new FormControl<string>('', Validators.required),
        solucao: new FormControl<string>('', Validators.required),
        idAutor: new FormControl<number>(0),
        dataAlteracao: new FormControl<Date>(new Date()),
    });

    constructor(private srv: IcConhecimentoService,private confirmationService: ConfirmationService) {
        effect(() => {
            if (this.ic()) {
                if (this.ic()) {

                }
            }
        });
    }

    atualiza() {
        if (this.ic()) {
            this.srv.ConhecimentosPorIC(this.ic()!.id).subscribe({
                next: r => {
                    this.lista.set(r);
                }
            });
        }
    }

    grava() {
        if (this.form.valid) {
            this.srv.Grava(this.form.value).subscribe({
                next: r => {
                    this.atualiza();
                }
            });
        }
    }

    exclui(id: number, event: any) {
        this.confirmationService.confirm({
            target: event.target,
            message: 'Tem certeza que deseja excluir este item?',
            icon: 'pi pi-exclamation-triangle',
            acceptLabel: 'Sim',
            rejectLabel: 'Não',
            rejectButtonStyleClass: 'p-button-danger',
            accept: () => {
                this.srv.Exclui(id).subscribe({
                    next: r => {
                        this.atualiza();
                    }
                });
            }
        });
    }

    novo() {
        this.form.reset();
        this.form.patchValue({
            id: 0,
            idIc: this.ic()?.id || 0,
            dataAlteracao: new Date()
        });
    }

    edita(item: IcConhecimento) {
        this.form.patchValue(item);
    }
}
