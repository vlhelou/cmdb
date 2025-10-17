import { Component, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TabsModule } from 'primeng/tabs';

// import { JsonPipe } from '@angular/common';
import { IcAutocompleteComponent } from 'src/app/ic/ic-autocomplete/ic-autocomplete.component';
import { IcTreeViewComponent } from 'src/app/ic/tree-view/tree-view.component';
import { CadastroComponent } from 'src/app/ic/cadastro/cadastro.component'
import { SegredoComponent } from 'src/app/ic/segredo/segredo.component'
import { ConhecimentoComponent } from 'src/app/ic/conhecimento/conhecimento.component'
import { icIc } from 'src/model/ic/ic';
import { IcService } from 'src/model/ic/ic.service';
import { DependenciaComponent } from 'src/app/ic/dependencia/dependencia.component'


@Component({
  selector: 'app-principal',
  standalone: true,
  imports: [
    IcAutocompleteComponent,
    FormsModule,
    IcTreeViewComponent,
    TabsModule,
    CadastroComponent,
    SegredoComponent,
    ConhecimentoComponent,
    DependenciaComponent
  ],
  templateUrl: './principal.component.html',
  styleUrl: './principal.component.scss'
})
export class PrincipalComponent {
  icAutocomplete: icIc | undefined = undefined;
  icTreeView: icIc | undefined = undefined;
  icAutoSelecionado: icIc | undefined = undefined;
  icTreeSelecionado: icIc | undefined = undefined;
  icSelecionado = signal<icIc | undefined>(undefined);
  icAtualiza = signal<icIc | undefined>(undefined);
  icNovoPai: icIc | undefined = undefined;
  tabSelecionado = "0";
  constructor(private srv: IcService) { }

  autoCompleteSelecionado(event: icIc | undefined) {
    this.icAutoSelecionado = event;
    this.icSelecionado.set(event);
  }

  icCadastroGravado(event: any) {
    this.icAtualiza.set(event);
  }

  mudaPaternidade() {
    this.srv.MudaPaternidade(this.icSelecionado()?.id!, this.icNovoPai?.id!).subscribe({
    });
  }

}
