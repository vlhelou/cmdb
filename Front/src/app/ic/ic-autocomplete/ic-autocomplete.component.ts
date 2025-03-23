import { Component } from '@angular/core';
import { AutoCompleteModule } from 'primeng/autocomplete';
import { IcService } from 'src/model/ic/ic.service';

@Component({
  selector: 'app-ic-autocomplete',
  standalone: true,
  imports: [AutoCompleteModule],
  templateUrl: './ic-autocomplete.component.html',
  styleUrl: './ic-autocomplete.component.scss'
})
export class IcAutocompleteComponent {
  constructor(private srv: IcService) { }
}
