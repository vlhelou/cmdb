import { Component, effect, input, signal, OnInit } from '@angular/core';
import { icIc } from 'src/model/ic/ic';
import { TableModule } from 'primeng/table';
import { FormControl, FormGroup, Validators, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { IcAutocompleteComponent } from 'src/app/ic/ic-autocomplete/ic-autocomplete.component'
import { DependenciaService } from 'src/model/ic/dependencia.service';
import { IcDependencia } from 'src/model/ic/dependencia';


@Component({
    selector: 'app-dependencia',
    imports: [TableModule, FormsModule, ReactiveFormsModule, IcAutocompleteComponent],
    templateUrl: './dependencia.component.html',
    styleUrl: './dependencia.component.scss'
})
export class DependenciaComponent implements OnInit {
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

    constructor(private srv: DependenciaService) {

        effect(() => {
            if (this.ic()) {
                if (this.ic()?.id) {
                    // console.log('Dependencia: ', this.ic());
                    const idic = this.ic()?.id || 0;
                    this.srv.DependenciasPorIC(idic, this.dependente()).subscribe({
                        next: (dados) => {
                            this.lista.set(dados);
                        }
                    });
                    // this.atualiza(idic);
                } else {
                    // this.lista.set([]);
                }
            }
        });

    }
    ngOnInit() {
    }

    grava() {
        const envio = this.form.value;
        envio.idIcPrincipal = this.ic()?.id || 0;
        envio.idIcDependente = envio.dependente?.id || 0;
        envio.idAutor = 0;
        envio.dataAlteracao = new Date();
        this.srv.Grava(envio).subscribe({
            next: (data) => {
                // console.log('Gravou dependencia: ', data);
                console.log('Gravar dependencia: ', envio);
            }
        });
        this.form.reset();
    }
}
