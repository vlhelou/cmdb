import { Component, effect, input, signal } from '@angular/core';
import { icIc } from 'src/model/ic/ic';
import { icSegredo } from 'src/model/ic/segredo'
import { IcSegredoService } from 'src/model/ic/segredo.service'
import { TableModule } from 'primeng/table';
import { PopoverModule } from 'primeng/popover';

@Component({
    selector: 'app-segredo',
    imports: [TableModule,PopoverModule],
    templateUrl: './segredo.component.html',
    styleUrl: './segredo.component.scss'
})
export class SegredoComponent {
    ic = input<icIc | undefined>();
    lista = signal<icSegredo[] >([]);
    conteudo = signal<string>('');

    constructor(private srv: IcSegredoService) {
        effect(() => {
            if (this.ic()) {
                if (this.ic()?.id) {
                    const idic = this.ic()?.id || 0;
                    this.srv.MeusSegredosPorIc(idic).subscribe({
                        next: (data) => {
                            this.lista.set(data);
                        }
                    });
                } else {
                    this.lista.set([]);
                }
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
}
