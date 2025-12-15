import { Component, effect, Input, input, output, signal } from '@angular/core';
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
    selecionado = output<icIc | undefined>();

    @Input() atualiza(): void {
        if (this.ic()) {
            this.srv.BuscaComFamilia(this.ic()!.id).subscribe({
                next: (ret) => {
                    this.familia.set(ret);
                }
            });
        }               
    }


    constructor(private srv: IcService) {
        
        effect(() => {
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

    selecionaIc(ic:icIc) {
        this.selecionado.emit(ic);
        return this.srv.BuscaComFamilia(ic.id).subscribe({
            next: (ret) => {
                this.familia.set(ret);
            }
        });
    }
}
