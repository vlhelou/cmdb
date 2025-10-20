import { Component, effect, input, signal } from '@angular/core';
import { icIc } from 'src/model/ic/ic';
import { TableModule } from 'primeng/table';
import { FormControl, FormGroup, Validators, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { IcAutocompleteComponent } from 'src/app/ic/ic-autocomplete/ic-autocomplete.component'
import { DependenciaService } from 'src/model/ic/dependencia.service';
import { IcDependencia } from 'src/model/ic/dependencia';
import { ConfirmPopupModule } from 'primeng/confirmpopup';
import { ConfirmationService  } from 'primeng/api';

@Component({
    selector: 'app-dependencia',
    imports: [TableModule, FormsModule, ReactiveFormsModule, IcAutocompleteComponent, ConfirmPopupModule],
    templateUrl: './dependencia.component.html',
    styleUrl: './dependencia.component.scss',
    providers: [ConfirmationService]
})
export class DependenciaComponent {
    ic = input<icIc | undefined>();
    dependente = input<boolean>(false);
    lista = signal<IcDependencia[]>([]);


    form = new FormGroup({
        id: new FormControl<number>(0),
        idIcPrincipal: new FormControl<number>(0),
        dependente: new FormControl<any>(null, [Validators.required]),
        idIcDependente: new FormControl<number | null>(null),
        idAutor: new FormControl<number>(0),
        dataAlteracao: new FormControl<Date>(new Date()),
        observacao: new FormControl<string | null>(null),
    });

    constructor(private srv: DependenciaService, private confirmationService: ConfirmationService) {
        effect(() => {
            if (this.ic()) {
                this.form.reset();
                if (this.ic()?.id) {
                    this.atualiza();
                }
            }
        });
    }

    atualiza() {
        const idic = this.ic()?.id || 0;
        this.srv.DependenciasPorIC(idic, this.dependente()).subscribe({
            next: (dados) => {
                this.lista.set(dados);
            }
        });

    }
    grava() {
        const envio = this.form.value;
        if (this.dependente()) {
            envio.idIcPrincipal = this.ic()?.id || 0;
            envio.idIcDependente = envio.dependente?.id || 0;
        } else {
            envio.idIcPrincipal = envio.dependente?.id || 0;
            envio.idIcDependente = this.ic()?.id || 0;
        }
        envio.idAutor = 0;
        envio.id = 0;
        envio.dataAlteracao = new Date();
        this.srv.Grava(envio).subscribe({
            next: (data) => {
                this.atualiza();
            }
        });
        this.form.reset();
    }



    exclui(item: IcDependencia, event: any) {
        console.log('Excluir dependencia: ', item);
        this.confirmationService.confirm({
            target: event.currentTarget as EventTarget,
            message: 'Confirma a exclusão desta dependência?',
            icon: 'pi pi-exclamation-triangle',
            acceptLabel: 'Sim',
            rejectLabel: 'Não',
            rejectButtonStyleClass: 'p-button-danger',

            accept: () => {
                this.srv.Exclui(item.id).subscribe({
                    next: () => {
                        const idic = this.ic()?.id || 0;
                        this.atualiza();
                    }
                });
            },
            reject: () => {
                // this.messageService.add({ severity: 'error', summary: 'Rejected', detail: 'You have rejected', life: 3000 });
            }
        });

    }
}
