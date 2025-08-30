import { Component } from '@angular/core';
import { OrgAutocompleteComponent } from 'src/app/organograma/org-autocomplete/org-autocomplete.component'
import {OrgTreeviewComponent} from 'src/app/organograma/org-treeview/org-treeview.component'


@Component({
  selector: 'app-principal',
  imports: [OrgAutocompleteComponent, OrgTreeviewComponent],
  templateUrl: './principal.component.html',
  styleUrl: './principal.component.scss'
})
export class OrganogramaPrincipalComponent {

}
