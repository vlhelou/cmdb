import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
// import { JsonPipe } from '@angular/common';
import { IcAutocompleteComponent } from 'src/app/ic/ic-autocomplete/ic-autocomplete.component';
import {IcTreeViewComponent} from 'src/app/ic/tree-view/tree-view.component';


@Component({
  selector: 'app-principal',
  standalone: true,
  imports: [IcAutocompleteComponent, FormsModule,IcTreeViewComponent],
  templateUrl: './principal.component.html',
  styleUrl: './principal.component.scss'
})
export class PrincipalComponent {
  teste: any;
  constructor() { }
}
