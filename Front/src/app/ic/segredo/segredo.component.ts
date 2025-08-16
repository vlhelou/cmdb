import { Component, input } from '@angular/core';
import { icIc } from 'src/model/ic/ic';

@Component({
  selector: 'app-segredo',
  imports: [],
  templateUrl: './segredo.component.html',
  styleUrl: './segredo.component.scss'
})
export class SegredoComponent {
  ic = input<icIc | undefined>();
}
