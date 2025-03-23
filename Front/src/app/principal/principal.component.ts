import { Component } from '@angular/core';
import {IcAutocompleteComponent} from 'src/app/ic/ic-autocomplete/ic-autocomplete.component';


@Component({
  selector: 'app-principal',
  standalone: true,
  imports: [IcAutocompleteComponent],
  templateUrl: './principal.component.html',
  styleUrl: './principal.component.scss'
})
export class PrincipalComponent {

  constructor() {}
}
