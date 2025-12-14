import { Component, OnInit, signal, output, forwardRef, input, effect, Input } from '@angular/core';
// import { JsonPipe, NgIf } from '@angular/common';
import { IcService } from 'src/model/ic/ic.service';
import { icIc } from 'src/model/ic/ic';
import { Nullable } from 'primeng/ts-helpers';
import { TreeModule } from 'primeng/tree';
import { TreeNode } from 'primeng/api';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';

@Component({
    selector: 'app-ic-tree-view',
    imports: [TreeModule],
    templateUrl: './tree-view.component.html',
    styleUrl: './tree-view.component.scss',
    providers: [
        {
            provide: NG_VALUE_ACCESSOR,
            useExisting: forwardRef(() => IcTreeViewComponent),
            multi: true
        }

    ]
})
export class IcTreeViewComponent implements OnInit, ControlValueAccessor {

    @Input() localizaItem(id: number): void {
        this.localizado = null;
        for (const item of this.nodes) {
            if (item.data.id === id) {
                console.log('Item encontrado na raiz:', item);
            } else {
                this.localizaRecursivoPorid(item, id);
                if (this.localizado) {
                    let parent = this.localizado.parent;
                    while (parent) {
                        parent.expanded = true;
                        parent = parent.parent;
                    }

                }
            }
        }
    }



    ic: icIc | Nullable = null;
    nodes: TreeNode[] = [];
    icSelecionado: any | Nullable = null;
    selecionado = output<icIc | undefined>();
    icNovo = input<icIc | undefined>();
    private localizado: any;


    localizaRecursivoPorid(item: any, id: number): void {
        if (item.children) {
            for (const filho of item.children) {
                if (filho.key === id.toString()) {
                    console.log('Item encontrado:', filho);
                    console.log('Item encontrado pai:', filho.parent);
                    this.localizado = filho;
                    return;
                }
                else {
                    if (filho) {
                        this.localizaRecursivoPorid(filho, id);
                    }
                }
            }
        }


    }

    writeValue(value: icIc | Nullable): void {
        this.ic = value;
    }

    registerOnChange(fn: (value: icIc | Nullable) => void): void {
        this.selecionado.subscribe(fn);
    }

    registerOnTouched(fn: () => void): void {
        // Implementar se necessário
    }
    constructor(private srv: IcService) {
        effect(() => {
            if (this.icNovo()) {
                // console.log('Atualiza Tree com o novo IC:', this.icNovo()?.id);
                this.atualizaTree();

                if (this.icNovo()?.id) {
                    const id = this.icNovo()?.id;
                }
            }
        });
    }

    ngOnInit(): void {
        this.atualizaTree();

    }

    atualizaTree(): void {
        this.srv.ListaCompleta().subscribe((data: any) => {
            this.ic = data;
            if (this.ic) {
                const menuTemp: TreeNode[] = [];
                const classe = this.ic.ativoFinal ? undefined : 'inativo';
                const tItem: TreeNode = {
                    label: this.ic.nome,
                    children: [],
                    data: this.ic,
                    key: this.ic.id.toString()
                };
                this.populaFilhos(this.ic, tItem);
                menuTemp.push(tItem);
                this.nodes = menuTemp;
            }
        });

    }

    private populaFilhos(origem: icIc, destino: TreeNode): void {
        if (origem.filhos) {
            origem.filhos.forEach(filho => {
                const classe = filho.ativoFinal ? undefined : 'inativo';
                const tFilho: TreeNode = { label: filho.nome, data: filho, key: filho.id.toString() };
                destino.children = destino.children || [];
                destino.children.push(tFilho);
                this.populaFilhos(filho, tFilho);
            });
        }
    }

    itemSelecionado(event: any) {
        this.selecionado.emit(event.node.data);
    }

}
