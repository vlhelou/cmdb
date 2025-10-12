import { Component, effect, input, signal, OnInit } from '@angular/core';
import { icIc } from 'src/model/ic/ic';
import { TableModule } from 'primeng/table';

@Component({
    selector: 'app-dependencia',
    imports: [TableModule],
    templateUrl: './dependencia.component.html',
    styleUrl: './dependencia.component.scss'
})
export class DependenciaComponent implements OnInit {
    ic = input<icIc | undefined>();

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
