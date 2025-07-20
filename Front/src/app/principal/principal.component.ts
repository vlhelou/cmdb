import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TabsModule } from 'primeng/tabs';
// import { JsonPipe } from '@angular/common';
import { IcAutocompleteComponent } from 'src/app/ic/ic-autocomplete/ic-autocomplete.component';
import { IcTreeViewComponent } from 'src/app/ic/tree-view/tree-view.component';
import { icIc } from 'src/model/ic/ic';

@Component({
  selector: 'app-principal',
  standalone: true,
  imports: [IcAutocompleteComponent, FormsModule, IcTreeViewComponent, TabsModule],
  templateUrl: './principal.component.html',
  styleUrl: './principal.component.scss'
})
export class PrincipalComponent {
  icAutocomplete: icIc|undefined = undefined;
  icTreeView: icIc|undefined = undefined;
  constructor() { }
}
