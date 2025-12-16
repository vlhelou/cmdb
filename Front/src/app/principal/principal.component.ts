import { Component, signal, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TabsModule } from 'primeng/tabs';

import { IcAutocompleteComponent } from 'src/app/ic/ic-autocomplete/ic-autocomplete.component';
import { FamiliaCompletaComponent } from 'src/app/ic/familia-completa/familia-completa.component'
import { CadastroComponent } from 'src/app/ic/cadastro/cadastro.component'
import { SegredoComponent } from 'src/app/ic/segredo/segredo.component'
import { ConhecimentoComponent } from 'src/app/ic/conhecimento/conhecimento.component'
import { icIc } from 'src/model/ic/ic';
import { IcService } from 'src/model/ic/ic.service';
import { DependenciaComponent } from 'src/app/ic/dependencia/dependencia.component'
import { PanelModule } from 'primeng/panel';


@Component({
    selector: 'app-principal',
    standalone: true,
    imports: [
        IcAutocompleteComponent,
        FormsModule,
        FamiliaCompletaComponent,
        PanelModule,
        TabsModule,
        CadastroComponent,
        SegredoComponent,
        ConhecimentoComponent,
        DependenciaComponent
    ],
    templateUrl: './principal.component.html',
    styleUrl: './principal.component.scss'
})
export class PrincipalComponent implements OnInit {
    icAutocomplete: icIc | undefined = undefined;
    icTreeView: icIc | undefined = undefined;
    icAutoSelecionado: icIc | undefined = undefined;
    icTreeSelecionado: icIc | undefined = undefined;
    icSelecionado = signal<icIc | undefined>(undefined);
    icAtualiza = signal<icIc | undefined>(undefined);
    icNovoPai: icIc | undefined = undefined;
    tabSelecionado = "0";
    usaEmbedding = signal<boolean>(false);
    pesquisaEmbeddingPrompt: string = "";
    icsPromptResultado=signal< icIc[]>([]); 
    constructor(private srv: IcService) { }

    ngOnInit(): void {
        this.srv.UsaEmbedding().subscribe({
            next: (ret) => {
                this.usaEmbedding.set(ret);
            }
        });
    }

    autoCompleteSelecionado(event: icIc | undefined) {
        this.icAutoSelecionado = event;
        this.icSelecionado.set(event);
    }

    icCadastroGravado(event: any, treeComponent: FamiliaCompletaComponent | null) {
        this.icAtualiza.set(event);
        if (treeComponent) {
            treeComponent.atualiza();
        }
    }

    mudaPaternidade() {
        this.srv.MudaPaternidade(this.icSelecionado()?.id!, this.icNovoPai?.id!).subscribe({
        });
    }

    pesquisaPrompt(){
        this.srv.PesquisaEmbedding(this.pesquisaEmbeddingPrompt).subscribe({
            next: (ret) => {
                this.icsPromptResultado.set(ret);
            }
        });
    }


}
