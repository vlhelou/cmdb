import { Component, effect, input, signal } from '@angular/core';
import { icIc } from 'src/model/ic/ic';
import { IcService } from 'src/model/ic/ic.service';

@Component({
    selector: 'app-ic-familia-completa',
    imports: [],
    templateUrl: './familia-completa.component.html',
    styleUrl: './familia-completa.component.scss',
})
export class FamiliaCompletaComponent {
    ic = input<icIc | undefined>();
    familia = signal<icIc | null>(null);


    constructor(private srv: IcService) {
        effect(() => {
            console.log(this.ic());
            if (this.ic()) {
                this.srv.BuscaComFamilia(this.ic()!.id).subscribe({
                    next: (ret) => {
                        this.familia.set(ret);
                    }
                });
            } else {
                this.srv.BuscaComFamilia(1).subscribe({
                    next: (ret) => {
                        this.familia.set(ret);
                    }
                });

            }
        });


    }

    selecionaIc(id: number) {
        return this.srv.BuscaComFamilia(id).subscribe({
            next: (ret) => {
                this.familia.set(ret);
            }
        });
    }
}
