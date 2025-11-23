import { Component, input, signal } from '@angular/core';
import { segOrganograma } from 'src/model/seg/organograma';


@Component({
  selector: 'app-equipe',
  imports: [],
  templateUrl: './equipe.component.html',
  styleUrl: './equipe.component.scss',
})
export class EquipeComponent {
  org = input<segOrganograma | undefined>();

  constructor() {

  }
}
