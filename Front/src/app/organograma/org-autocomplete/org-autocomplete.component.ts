import { Component, forwardRef, input, output, signal } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { FormsModule } from '@angular/forms';
import { AutoCompleteModule } from 'primeng/autocomplete';
import { OrganogramaService } from 'src/model/seg/organograma.service';
import { segOrganograma } from 'src/model/seg/organograma'

@Component({
  selector: 'app-org-autocomplete',
  imports: [AutoCompleteModule, FormsModule],
  templateUrl: './org-autocomplete.component.html',
  styleUrl: './org-autocomplete.component.scss',
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => OrgAutocompleteComponent),
      multi: true
    }
  ]

})
export class OrgAutocompleteComponent implements ControlValueAccessor {
  value: segOrganograma | null = null;
  isDisabled: boolean = false;
  lista = signal(Array<segOrganograma>());
  ativo = input<boolean | undefined>();
  filhoDe = input<string | undefined>();
  idTipo = input<number | undefined>();
  selecionado = output<any | undefined>();

  private onChange: any = () => { }
  private onTouched: any = () => { }

  constructor(private srv: OrganogramaService) { }

  onSelect(event: any) {
    this.value = event.value;
    this.onChange(event.value);
    this.onTouched();
    this.selecionado.emit(event.value);

  }

  onUnselect(event: any) {
    if (this.value == null) {
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
