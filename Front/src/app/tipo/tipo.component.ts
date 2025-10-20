import { Component, OnInit, signal } from '@angular/core';
import { TipoService } from 'src/model/corp/tipo.service';
import { corpTipo } from 'src/model/corp/tipo';
import { TableModule } from 'primeng/table';
import { FormControl, FormGroup, Validators, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ConfirmPopupModule } from 'primeng/confirmpopup';
import { ConfirmationService } from 'primeng/api';
import { CheckboxModule } from 'primeng/checkbox';

@Component({
    selector: 'app-tipo',
    imports: [TableModule, FormsModule, ReactiveFormsModule, ConfirmPopupModule, CheckboxModule],
    templateUrl: './tipo.component.html',
    styleUrl: './tipo.component.scss',
    providers: [ConfirmationService]
})
export class TipoComponent implements OnInit {

    tipos = signal<corpTipo[]>([]);

    form = new FormGroup({
        id: new FormControl<number>(0),
        nome: new FormControl<string | null>(null, [Validators.required]),
        grupo: new FormControl<string | null>(null, [Validators.required]),
        ativo: new FormControl<boolean | null>(null, [Validators.required]),
    });


    constructor(private srv: TipoService, private confirmationService: ConfirmationService) { }

    ngOnInit(): void {
        this.atualiza();
    }

    atualiza() {
        this.srv.Lista().subscribe({
            next: (data) => {
                this.tipos.set(data);
            }
        });

    }


    exclui(item: corpTipo, event: any) {
        this.confirmationService.confirm({
            target: event.currentTarget as EventTarget,
            message: 'Confirma a exclusão deste tipo?',
            icon: 'pi pi-exclamation-triangle',
            acceptLabel: 'Sim',
            rejectLabel: 'Não',
            rejectButtonStyleClass: 'p-button-danger',

            accept: () => {
                this.srv.Exclui(item.id).subscribe({
                    next: () => {
                        this.atualiza();
                    }
                });
            },
            reject: () => {
                // this.messageService.add({ severity: 'error', summary: 'Rejected', detail: 'You have rejected', life: 3000 });
            }
        });

    }

    edita(item: any) {
        this.form.patchValue(item);
    }

    grava() {
        this.srv.Grava(this.form.value).subscribe({
            next: (data) => {
                this.atualiza();
                this.form.reset();
            }
        });
    }

}
