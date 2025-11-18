import { Component, OnInit, signal } from '@angular/core';
import { corpConfiguracao } from 'src/model/corp/configuracao'
import { ConfiguracaoService } from 'src/model/corp/configuracao.service'
import { TreeModule } from 'primeng/tree';
import { TreeNode } from 'primeng/api';
import { FormsModule } from '@angular/forms';
import { CheckboxModule } from 'primeng/checkbox';

@Component({
    selector: 'app-configuracao',
    imports: [TreeModule, FormsModule, CheckboxModule],
    templateUrl: './configuracao.component.html',
    styleUrl: './configuracao.component.scss'
})
export class ConfiguracaoComponent implements OnInit {
    origemArvore: corpConfiguracao[] = [];
    configuracaoSelecionada: corpConfiguracao | null = null;
    arvore = signal<TreeNode[]|undefined>(undefined);
    selecionado: TreeNode | null = null;

    constructor(private srv: ConfiguracaoService) { }

    ngOnInit() {
        this.srv.ArvoreCompleta().subscribe({
            next: data => {
                this.origemArvore = data;
                this.origemArvore.forEach((item) => {
                    const novo = [{
                        label: item.nome,
                        data: item,
                        children: this.montaArvore(item),
                    }]
                    // console.log(novo);
                    this.arvore.set(novo);
                });
            }
        });
    }


    montaArvore(item: corpConfiguracao): TreeNode[] {
        let retorno: TreeNode[] = [];
        if (item.filhos) {
            item.filhos.forEach((filho) => {
                retorno.push({
                    label: filho.nome,
                    data: filho,
                    children: this.montaArvore(filho),
                });
            });
        }
        return retorno;
    }
    
    itemSelecionado(event:any) {
        this.configuracaoSelecionada = event.node.data;
    }
    
    itemDesselecionado() {
        this.selecionado = null;
        this.configuracaoSelecionada = null;
    }


    salvar() {
        this.srv.GravaValor(this.configuracaoSelecionada!).subscribe({
            next: data => {
                this.selecionado!.data = data;
                this.configuracaoSelecionada = data;
            },
        });        
    }

}
