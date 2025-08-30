import { Component, OnInit, signal, output, forwardRef, input, effect } from '@angular/core';
import { OrganogramaService } from 'src/model/seg/organograma.service';
import { segOrganograma } from 'src/model/seg/organograma';
import { Nullable } from 'primeng/ts-helpers';
import { TreeModule } from 'primeng/tree';
import { TreeNode } from 'primeng/api';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';


@Component({
  selector: 'app-org-treeview',
  imports: [TreeModule],
  templateUrl: './org-treeview.component.html',
  styleUrl: './org-treeview.component.scss',
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => OrgTreeviewComponent),
      multi: true
    }

  ]

})
export class OrgTreeviewComponent implements OnInit, ControlValueAccessor {

  organograma: segOrganograma | Nullable = null;
  nodes = signal<TreeNode[]>([]);
  icSelecionado: any | Nullable = null;
  selecionado = output<segOrganograma | undefined>();
  icNovo = input<segOrganograma | undefined>();


  writeValue(value: segOrganograma | Nullable): void {
    this.organograma = value;
  }

  registerOnChange(fn: (value: segOrganograma | Nullable) => void): void {
    this.selecionado.subscribe(fn);
  }

  registerOnTouched(fn: () => void): void {
    // Implementar se necessário
  }
  constructor(private srv: OrganogramaService) {
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
      this.organograma = data;
      if (this.organograma) {
        const menuTemp: TreeNode[] = [];
        const classe = this.organograma.ativoFinal ? undefined : 'inativo';
        const tItem: TreeNode = {
          label: this.organograma.nome,
          children: [],
          data: this.organograma,
          styleClass: classe
        };
        this.PopulaFilhos(this.organograma, tItem);
        menuTemp.push(tItem);
        this.nodes.set(menuTemp);

      }
    });

  }

  private PopulaFilhos(origem: segOrganograma, destino: TreeNode): void {
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
