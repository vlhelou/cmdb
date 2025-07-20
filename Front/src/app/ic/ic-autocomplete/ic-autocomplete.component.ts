import { Component, forwardRef, input, output, signal } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { FormsModule } from '@angular/forms';
import { AutoCompleteModule } from 'primeng/autocomplete';
import { IcService } from 'src/model/ic/ic.service';
import { icIc } from 'src/model/ic/ic';

@Component({
  selector: 'app-ic-autocomplete',
  standalone: true,
  imports: [AutoCompleteModule, FormsModule],
  templateUrl: './ic-autocomplete.component.html',
  styleUrl: './ic-autocomplete.component.scss',
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => IcAutocompleteComponent),
      multi: true
    }
  ]
})
export class IcAutocompleteComponent implements ControlValueAccessor {

  value: icIc|null = null;
  isDisabled: boolean = false;
  lista = signal(Array<icIc>());
  ativo = input<boolean | undefined>();
  filhoDe = input<string | undefined>();
  idTipo = input<number | undefined>();
  selecionado = output<any | undefined>();

  private onChange: any = () => {}
  private onTouched: any = () => {}

  constructor(private srv: IcService) { }

  onSelect(event: any) {
    this.value = event.value;
    this.onChange(event.value);
    this.onTouched();
    this.selecionado.emit(event.value);
    
  }

  onUnselect(event: any) {
    if (this.value==null){
      this.value = null;
      this.onChange(null);
    }
    this.selecionado.emit(this.value);
    this.onTouched();
  }

  writeValue(value: any): void {
    this.value = value;
  }
  registerOnChange(fn: any): void {
    this.onChange = fn;
  }
  registerOnTouched(fn: any): void {
    this.onTouched = fn;
  }
  setDisabledState?(isDisabled: boolean): void {
  }


  pesquisa(event: any) {
    const prm = {
      chave: event.query,
      idTipo: this.idTipo() ? this.idTipo() : null,
      ativo: this.ativo() ? this.ativo() : null,
      filhoDe: this.filhoDe() ? this.filhoDe() : null
    };
    this.srv.Pesquisa(prm).subscribe({
      next: (data) => {
        this.lista.set(data);
      }
    });
  }


}
