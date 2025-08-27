import { Component, effect, input, signal, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { icIc } from 'src/model/ic/ic';
import { icSegredo } from 'src/model/ic/segredo'
import { IcSegredoService } from 'src/model/ic/segredo.service'
import { TableModule } from 'primeng/table';
import { PopoverModule } from 'primeng/popover';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { ConfirmationService } from 'primeng/api';
import { DialogModule } from 'primeng/dialog';
import { segUsuarioService } from 'src/model/seg/usuario.service';
import { segOrganograma } from 'src/model/seg/organograma'
import { SelectModule } from 'primeng/select';

@Component({
    selector: 'app-segredo',
    imports: [TableModule, PopoverModule, ConfirmDialogModule, DialogModule, FormsModule, ReactiveFormsModule, SelectModule],
    templateUrl: './segredo.component.html',
    styleUrl: './segredo.component.scss',
    providers: [ConfirmationService]
})
export class SegredoComponent implements OnInit {
    ic = input<icIc | undefined>();
    lista = signal<icSegredo[]>([]);
    conteudo = signal<string>('');
    organogramas = signal<segOrganograma[]>([]);
    mostraNovo = false;

    formNovo = new FormGroup({
        idOrganogramaDono: new FormControl<number | null>(null),
        conteudo: new FormControl('', Validators.required),
    });

    constructor(private srv: IcSegredoService, private confirmationService: ConfirmationService, private usuario: segUsuarioService) {
        effect(() => {
            if (this.ic()) {
                if (this.ic()?.id) {
                    const idic = this.ic()?.id || 0;
                    this.atualiza(idic);
                } else {
                    this.lista.set([]);
                }
            }
        });
    }

    ngOnInit(): void {
        this.usuario.MeusOrganogramas().subscribe({
            next: (data) => {
                const eu = {
                    id: 0,
                    nomeCompleto: 'Eu',
                    idPai: 0,
                    nome: 'Eu',
                    ativo: true,
                    ativoFinal: true,
                    listaAncestrais: '',
                    nivel: 0,
                    lstAncestrais: [],
                    pai: null,
                    filhos: null,
                    ancestrais: null
                }
                this.organogramas.set([eu, ...data]);
            }
        });
    }

    private atualiza(id: number) {
        this.srv.MeusSegredosPorIc(id).subscribe({
            next: (data) => {
                this.lista.set(data);
            }
        });

    }


    mostraConteudo(event: any, op: any, id: number) {
        this.srv.Visualiza(id).subscribe({
            next: (data) => {
                op.toggle(event)
                this.conteudo.set(data.conteudo || '');
                // Manipule os dados recebidos conforme necessário
            }
        });

    }

    confirmaExclusao(event: any, id: number) {
        this.confirmationService.confirm({
            target: event.target,
            message: 'Tem certeza que deseja excluir este item?',
            icon: 'pi pi-exclamation-triangle',
            acceptLabel: 'Sim',
            rejectLabel: 'Não',
            rejectButtonStyleClass: 'p-button-danger',
            accept: () => {
                this.srv.Exclui(id).subscribe({
                    next: () => {
                        this.atualiza(this.ic()?.id || 0);
                    }
                });
            }
        });
    }

    novo(){
        this.mostraNovo = true;
        this.formNovo.reset();
    }

    salvar() {
        if (this.formNovo.valid) {
            this.srv.novo({...this.formNovo.value, idIc: this.ic()?.id}).subscribe({
                next: () => {
                    this.atualiza(this.ic()?.id || 0);
                    this.mostraNovo = false;
                }
            });
        }
    }

}
