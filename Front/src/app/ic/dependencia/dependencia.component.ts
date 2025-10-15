import { Component, effect, input, signal, OnInit } from '@angular/core';
import { icIc } from 'src/model/ic/ic';
import { TableModule } from 'primeng/table';
import { FormControl, FormGroup, Validators, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { IcAutocompleteComponent } from 'src/app/ic/ic-autocomplete/ic-autocomplete.component'
import { IcDependencia } from 'src/model/ic/dependencia';

@Component({
    selector: 'app-dependencia',
    imports: [TableModule, FormsModule, ReactiveFormsModule, IcAutocompleteComponent],
    templateUrl: './dependencia.component.html',
    styleUrl: './dependencia.component.scss'
})
export class DependenciaComponent implements OnInit {
    ic = input<icIc | undefined>();
    dependencia = input<boolean>(false);


    form = new FormGroup({
        id: new FormControl<number>(0),
        idPrincipal: new FormControl<number>(0),
        idDependente: new FormControl<number>(0, Validators.required),
        dependente: new FormControl<any>(null),
        idAutor: new FormControl<number>(0, Validators.required),
        dataAlteracao: new FormControl<Date>(new Date()),
        observacao: new FormControl<string>(''),
    });


    ngOnInit() {
        effect(() => {
            if (this.ic()) {
                if (this.ic()?.id) {
                    const idic = this.ic()?.id || 0;
                    // this.atualiza(idic);
                } else {
                    // this.lista.set([]);
                }
            }
        });
    }
}
