import { Component, effect, input, signal } from '@angular/core';
import { icIc } from 'src/model/ic/ic';
import { icSegredo } from 'src/model/ic/segredo'
import { IcSegredoService } from 'src/model/ic/segredo.service'


@Component({
    selector: 'app-segredo',
    imports: [],
    templateUrl: './segredo.component.html',
    styleUrl: './segredo.component.scss'
})
export class SegredoComponent {
    ic = input<icIc | undefined>();
    lista = signal<icSegredo[] | null>(null);
    constructor(private icSegredoService: IcSegredoService) {
        effect(() => {
            if (this.ic()) {
                if (this.ic()?.id) {
                    const idic = this.ic()?.id || 0;
                    this.icSegredoService.MeusSegredosPorIc(idic).subscribe({
                        next: (data) => {
                            console.log(data);
                        }
                    });
                } else {
                    this.lista.set(null);
                }
            }
        });
    }
}
