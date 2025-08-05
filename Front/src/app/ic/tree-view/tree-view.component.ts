import { Component, OnInit, signal, output, forwardRef, input, effect } from '@angular/core';
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

  ic: icIc | Nullable = null;
  nodes = signal<TreeNode[]>([]);
  icSelecionado: any | Nullable = null;
  selecionado = output<icIc | undefined>();
  icNovo = input<icIc | undefined>();


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
          console.log('Atualiza Tree com o novo IC:', id);
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
          styleClass: classe
        };
        this.PopulaFilhos(this.ic, tItem);
        menuTemp.push(tItem);
        this.nodes.set(menuTemp);

      }
    });

  }

  private PopulaFilhos(origem: icIc, destino: TreeNode): void {
    if (origem.filhos) {
      origem.filhos.forEach(filho => {
        const classe = filho.ativoFinal ? undefined : 'inativo';
        const tFilho: TreeNode = { label: filho.nome, data: filho, styleClass: classe };
        destino.children = destino.children || [];
        destino.children.push(tFilho);
        this.PopulaFilhos(filho, tFilho);
      });
    }
  }

  itemSelecionado(event: any) {
    this.selecionado.emit(event.node.data);
  }

}
