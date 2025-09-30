import { Component, OnInit } from '@angular/core';
import { FormGroup, Validators, FormControl, FormsModule, ReactiveFormsModule, ValidatorFn, AbstractControl, ValidationErrors } from '@angular/forms';
import { Router } from '@angular/router';
import { PasswordModule } from 'primeng/password';
import { segUsuarioService } from 'src/model/seg/usuario.service';


@Component({
    selector: 'app-recuperacao-senha',
    imports: [FormsModule, ReactiveFormsModule, PasswordModule],
    templateUrl: './recuperacao-senha.component.html',
    styleUrl: './recuperacao-senha.component.scss'
})
export class RecuperacaoSenhaComponent implements OnInit {
    form: FormGroup;

    constructor(private srv: segUsuarioService, private router: Router,) {
        this.form = new FormGroup({
            chave: new FormControl<string>('', [Validators.required, Validators.minLength(2)]),
            senha: new FormControl<string>('', [Validators.required, Validators.minLength(2)]),
            confirmacao: new FormControl<string>('', [Validators.required, Validators.minLength(2)]),
        }, { validators: this.confirmacaoSenhaRepetida() });

    }

    ngOnInit(): void {

    }

    confirmacaoSenhaRepetida(): ValidatorFn {
        return (control: AbstractControl): ValidationErrors | null => {
            const senha = control.get('senha');
            const confirmacao = control.get('confirmacao');

            if (!senha || !confirmacao) {
                return null;
            }

            if (confirmacao.errors && !confirmacao.errors['passwordMismatch']) {
                return null;
            }

            if (senha.value !== confirmacao.value) {
                return { passwordMismatch: true };
            }

            return null;
        };
    }

    ajustaSenha() {
        if (this.form.valid) {
            this.srv.RecuperaSenhaChave(this.form.value).subscribe({
                next: () => {
                    this.router.navigate(['/home']);
                },
            });
        }
     }

}


