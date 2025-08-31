import { Component, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { OrgAutocompleteComponent } from 'src/app/organograma/org-autocomplete/org-autocomplete.component'
import { OrgTreeviewComponent } from 'src/app/organograma/org-treeview/org-treeview.component'
import { TabsModule } from 'primeng/tabs';
import { CadastroComponent } from 'src/app/organograma/cadastro/cadastro.component'
import { segOrganograma } from 'src/model/seg/organograma'
import { OrganogramaService } from 'src/model/seg/organograma.service'


@Component({
  selector: 'app-principal',
  imports: [OrgAutocompleteComponent, OrgTreeviewComponent, TabsModule, CadastroComponent, FormsModule],
  templateUrl: './principal.component.html',
  styleUrl: './principal.component.scss'
})
export class OrganogramaPrincipalComponent {

  orgSelecionado = signal<segOrganograma | undefined>(undefined);
  orgAutoSelecionado: segOrganograma | undefined = undefined;
  orgTreeSelecionado: segOrganograma | undefined = undefined;
  orgAtualiza = signal<segOrganograma | undefined>(undefined);
  orgNovoPai: segOrganograma | undefined = undefined;

  constructor(private srv: OrganogramaService) { }

  autoCompleteSelecionado(event: segOrganograma | undefined) {
    this.orgAutoSelecionado = event;
    this.orgSelecionado.set(event);
  }

  orgCadastroGravado(event: any) {
    this.orgAtualiza.set(event);
  }

  mudaPaternidade() {
    const novoPai = this.orgNovoPai;
    if (novoPai) {
      this.srv.MudaPaternidade(this.orgSelecionado()?.id!, this.orgNovoPai?.id!).subscribe({
        next: (data) => {
          this.orgAtualiza.set(data);
        }
      });
    }
  }

}
