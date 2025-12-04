import { Component, forwardRef, input, output, signal } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { FormsModule } from '@angular/forms';
import { AutoCompleteModule } from 'primeng/autocomplete';
import { segUsuario } from 'src/model/seg/usuario';
import { segUsuarioService } from 'src/model/seg/usuario.service';


@Component({
  selector: 'app-usuario-auto-complete',
  imports: [AutoCompleteModule, FormsModule],
  templateUrl: './auto-complete.component.html',
  styleUrl: './auto-complete.component.scss',
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => UsuarioAutoCompleteComponent),
      multi: true
    }
  ]

})
export class UsuarioAutoCompleteComponent implements ControlValueAccessor {
  value: segUsuario | null = null;
  isDisabled: boolean = false;
  lista = signal(Array<segUsuario>());
  ativo = input<boolean | undefined>();
  filhoDe = input<string | undefined>();
  idTipo = input<number | undefined>();
  selecionado = output<any | undefined>();

  private onChange: any = () => { }
  private onTouched: any = () => { }

  constructor(private srv: segUsuarioService) { }

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
    };
    this.srv.Pesquisa(prm).subscribe({
      next: (data:any) => {
        this.lista.set(data);
      }
    });
  }

}
