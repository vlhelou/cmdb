import { Component, OnInit, input, signal } from '@angular/core';
import { FormGroup, Validators, FormArray, FormControl, FormsModule ,ReactiveFormsModule } from '@angular/forms';
import { ConfirmationService } from 'primeng/api';
import { MessageService } from 'primeng/api';
import { icIc } from 'src/model/ic/ic';

@Component({
  selector: 'app-ic-cadastro',
  imports: [FormsModule, ReactiveFormsModule],
  templateUrl: './cadastro.component.html',
  styleUrl: './cadastro.component.scss',
  providers: [ConfirmationService, MessageService]
})
export class CadastroComponent implements OnInit {

  ic = input<icIc | undefined>();
  frmIC = new FormGroup({
    Id: new FormControl<number>(0),
    IdPai: new FormControl<number | null>(null),
    Nome: new FormControl<string>('', [Validators.required, Validators.minLength(2)]),
    Ativo: new FormControl<boolean>(true),
    AtivoFinal: new FormControl<boolean>({ value: true, disabled: true }),
    Tipo: new FormControl<string>('', [Validators.required]),
    Responsavel: new FormControl<string | null>(null),
    Propriedades: new FormArray([])
  });

  frmPropriedade = new FormGroup({
    Nome: new FormControl<string>('', [Validators.required]),
    Valor: new FormControl<string>('', [Validators.required])
  });

  constructor() {

  }
  ngOnInit(): void {

  }

  grava(){}

}
