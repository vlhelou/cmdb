import { Component, OnInit, input, signal } from '@angular/core';
import { FormGroup, Validators, FormArray, FormControl, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { JsonPipe } from '@angular/common';
import { ConfirmationService } from 'primeng/api';
import { MessageService } from 'primeng/api';
import { TipoService } from 'src/model/corp/tipo.service'
import { corpTipo } from 'src/model/corp/tipo';
import { IcService } from 'src/model/ic/ic.service'
import { icIc } from 'src/model/ic/ic';
import { SelectModule } from 'primeng/select';
import { InputTextModule } from 'primeng/inputtext';
import { CheckboxModule } from 'primeng/checkbox';

@Component({
  selector: 'app-ic-cadastro',
  imports: [FormsModule, ReactiveFormsModule, SelectModule, InputTextModule, CheckboxModule, JsonPipe],
  templateUrl: './cadastro.component.html',
  styleUrl: './cadastro.component.scss',
  providers: [ConfirmationService, MessageService]
})
export class CadastroComponent implements OnInit {

  ic = input<icIc | undefined>();
  tipos = signal<corpTipo[]>([]);
  frmIC = new FormGroup({
    id: new FormControl<number>(0),
    idPai: new FormControl<number | null>(null),
    nome: new FormControl<string>('', [Validators.required, Validators.minLength(2)]),
    ativo: new FormControl<boolean>(true),
    ativoFinal: new FormControl<boolean>({ value: true, disabled: true }),
    ativoPai: new FormControl<boolean>({ value: true, disabled: true }),
    idTipo: new FormControl<number | null>(null, [Validators.required]),
    responsavel: new FormControl<string | null>(null),
    propriedades: new FormArray([
      new FormGroup({
        nome: new FormControl<string>('', [Validators.required]),
        valor: new FormControl<string>('', [Validators.required]),
      })
    ]),
  });


  constructor(private srv: IcService, private tipo: TipoService) { }

  ngOnInit(): void {
    this.icPropriedades.controls=[];
    this.tipo.ListaAtivos('tipo').subscribe({
      next: (data) => {
        this.tipos.set(data);
      },
    });


  }

  get icPropriedades(): FormArray {
    return this.frmIC.get('propriedades') as FormArray;
  }
  propriedadeNova() {
    const propriedade = new FormGroup({
      nome: new FormControl<string>('', [Validators.required]),
      valor: new FormControl<string>('', [Validators.required])
    });
    this.icPropriedades.push(propriedade);
    return propriedade;
  }

  propriedadeRemove(index: number) {
    this.icPropriedades.removeAt(index);
  }

  grava() { }

}
