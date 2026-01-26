import { Component, effect, input, signal, OnInit, output } from '@angular/core';
import { DatePipe } from '@angular/common';
import { icIc } from 'src/model/ic/ic';
import { IcConhecimento } from 'src/model/ic/conhecimento';
import { IcConhecimentoService } from 'src/model/ic/conhecimento.service';
import { FormControl, FormGroup, Validators, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { ConfirmationService } from 'primeng/api';
import { TableModule } from 'primeng/table';
import { DialogModule } from 'primeng/dialog';

@Component({
    selector: 'app-conhecimento',
    imports: [
        FormsModule,
        ReactiveFormsModule,
        ConfirmDialogModule,
        TableModule,
        DatePipe,
        DialogModule
    ],
    templateUrl: './conhecimento.component.html',
    styleUrl: './conhecimento.component.scss',
    providers: [ConfirmationService]
})
export class ConhecimentoComponent {
    ic = input<icIc | undefined>();
    lista = signal<IcConhecimento[]>([]);
    mostraFormulario = false;
    quantidade = output<number>();


    form = new FormGroup({
        id: new FormControl<number>(0),
        idIc: new FormControl<number>(0),
        problema: new FormControl<string>('', Validators.required),
        solucao: new FormControl<string>('', Validators.required),
        idAutor: new FormControl<number>(0),
        dataAlteracao: new FormControl<Date>(new Date()),
    });

    constructor(private srv: IcConhecimentoService, private confirmationService: ConfirmationService) {
        effect(() => {
            if (this.ic()) {
                this.atualiza();
            }
        });
    }

    atualiza() {
        if (this.ic()) {
            this.srv.ConhecimentosPorIC(this.ic()!.id).subscribe({
                next: r => {
                    this.lista.set(r);
                    this.quantidade.emit(r.length);
                }
            });
        }
    }

    grava() {
        if (this.form.valid) {
            this.srv.Grava(this.form.value).subscribe({
                next: r => {
                    this.mostraFormulario = false;
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
            idAutor: 0,
            dataAlteracao: new Date()
        });
        this.mostraFormulario = true;
    }

    edita(item: IcConhecimento) {
        this.form.reset();
        this.form.patchValue(item);
        this.form.controls['idIc'].setValue(item.idIc);
        this.mostraFormulario = true;
    }
}
