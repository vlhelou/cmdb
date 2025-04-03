import { Component, OnInit, signal, output } from '@angular/core';
// import { JsonPipe, NgIf } from '@angular/common';
import { IcService } from 'src/model/ic/ic.service';
import { icIc } from 'src/model/ic/ic';
import { Nullable } from 'primeng/ts-helpers';
import { TreeModule } from 'primeng/tree';
import { TreeNode } from 'primeng/api';

@Component({
  selector: 'app-ic-tree-view',
  imports: [TreeModule],
  templateUrl: './tree-view.component.html',
  styleUrl: './tree-view.component.scss'
})
export class IcTreeViewComponent implements OnInit {

  ic: icIc | Nullable = null;
  nodes = signal<TreeNode[]>([]);
  icSelecionado: icIc | Nullable = null;
  selecionado = output<icIc | undefined>();
  constructor(private srv: IcService) { }

  ngOnInit(): void {
    this.atualizaTree();
  }
  
  atualizaTree(){
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
