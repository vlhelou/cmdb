import { Component, signal, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TabsModule } from 'primeng/tabs';
import { ActivatedRoute } from '@angular/router';

import { IcAutocompleteComponent } from 'src/app/ic/ic-autocomplete/ic-autocomplete.component';
import { FamiliaCompletaComponent } from 'src/app/ic/familia-completa/familia-completa.component'
import { CadastroComponent } from 'src/app/ic/cadastro/cadastro.component'
import { SegredoComponent } from 'src/app/ic/segredo/segredo.component'
import { ConhecimentoComponent } from 'src/app/ic/conhecimento/conhecimento.component'
import { icIc } from 'src/model/ic/ic';
import { IcService } from 'src/model/ic/ic.service';
import { DependenciaComponent } from 'src/app/ic/dependencia/dependencia.component'
import { PanelModule } from 'primeng/panel';
import { BadgeModule } from 'primeng/badge';
declare var webkitSpeechRecognition: any;

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
        DependenciaComponent,
        BadgeModule
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
    estiloMicrofone = signal<any>({ "font-size": "20px", "color": "black" });
    pesquisaEmbeddingPrompt: string = "";
    icsPromptResultado = signal<icIc[]>([]);
    quantidadeSegredos = signal<number>(0);
    quantidadeConhecimentos = signal<number>(0);
    recognition = new webkitSpeechRecognition();
    constructor(private srv: IcService, private route: ActivatedRoute) {
        this.recognition.interimResults = true;
        this.recognition.lang = 'pt-BR';
    }

    ngOnInit(): void {
        const pid = this.route.snapshot.paramMap.get('id');
        const id = parseInt(pid || '');
        if (id) {
            this.srv.BuscaComFamilia(id).subscribe({
                next: (ret) => {
                    this.icSelecionado.set(ret);
                }
            });
        }
        this.srv.UsaEmbedding().subscribe({
            next: (ret) => {
                this.usaEmbedding.set(ret);
            }
        });

        this.recognition.addEventListener('result', (event: any) => {
            const transcript = Array.from(event.results)
                .map((result: any) => result[0])
                .map((result: any) => result.transcript)
                .join('');
            this.pesquisaEmbeddingPrompt = transcript;
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

    pesquisaPrompt() {
        this.srv.PesquisaEmbedding(this.pesquisaEmbeddingPrompt).subscribe({
            next: (ret) => {
                this.icsPromptResultado.set(ret);
            }
        });
    }

    retornoQuantidadeSegredos(event: any) {
        this.quantidadeSegredos.set(event);
    }

    retornoQuantidadeConhecimentos(event: any) {
        this.quantidadeConhecimentos.set(event);
    }

    iniciaAudio() {
        this.estiloMicrofone.set({ "font-size": "20px", "color": "red" });
        this.recognition.start();
        // console.log('Speech recognition started');

        this.recognition.addEventListener('end', () => {
            this.recognition.stop();
            // console.log('End speech recognition');
            this.estiloMicrofone.set({ "font-size": "20px", "color": "black" });
            this.pesquisaPrompt();

        });

    }

}
