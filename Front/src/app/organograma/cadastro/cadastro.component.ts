import { Component, OnInit, effect, input, output, signal } from '@angular/core';
import { FormGroup, Validators, FormArray, FormControl, FormsModule, ReactiveFormsModule } from '@angular/forms';
// import { JsonPipe } from '@angular/common';
import { ConfirmationService } from 'primeng/api';
import { MessageService } from 'primeng/api';
// import { TipoService } from 'src/model/corp/tipo.service'
// import { corpTipo } from 'src/model/corp/tipo';
import { segOrganograma } from 'src/model/seg/organograma'
import { OrganogramaService } from 'src/model/seg/organograma.service'
import { SelectModule } from 'primeng/select';
import { InputTextModule } from 'primeng/inputtext';
import { CheckboxModule } from 'primeng/checkbox';
import { ToastModule } from 'primeng/toast';


@Component({
    selector: 'app-cadastro',
    imports: [FormsModule, ReactiveFormsModule, SelectModule, InputTextModule, CheckboxModule, ToastModule],
    templateUrl: './cadastro.component.html',
    styleUrl: './cadastro.component.scss',
    providers: [ConfirmationService, MessageService]
})
export class CadastroComponent {

    org = input<segOrganograma | undefined>();
    orgPai = input<segOrganograma | undefined>();
    novo = input<boolean | undefined>();
    gravado = output<any | undefined>();
    frmOrg = new FormGroup({
        id: new FormControl<number>(0),
        idPai: new FormControl<number | null>(null),
        nome: new FormControl<string>('', [Validators.required, Validators.minLength(2)]),
        ativo: new FormControl<boolean>(true),
        ativoFinal: new FormControl<boolean>({ value: true, disabled: true }),
    });


    constructor(private srv: OrganogramaService, private messageService: MessageService) {
        effect(() => {
            if (this.org()) {
                this.frmOrg.patchValue({
                    id: this.org()?.id,
                    idPai: this.org()?.idPai,
                    nome: this.org()?.nome,
                    ativo: this.org()?.ativo,
                    ativoFinal: this.org()?.ativoFinal,
                });
            } else {
                this.frmOrg.reset({
                    id: 0,
                    idPai: null,
                    nome: '',
                    ativo: true,
                    ativoFinal: true,
                });
            }
        });
    }

    grava() {
        const icData = this.frmOrg.value;
        if (this.novo()) {
            icData.idPai = this.orgPai() ? this.orgPai()?.id : null;
        }

        this.srv.Grava(icData).subscribe({
            next: (data) => {
                this.messageService.add({ severity: 'success', summary: 'Sucesso', detail: 'Organograma salvo com sucesso!' });
                this.gravado.emit(data);
                this.frmOrg.reset();
            }
        });
    }
}
