import { Component, OnInit, signal } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { PopoverModule } from 'primeng/popover';
import { CheckboxModule } from 'primeng/checkbox';
import { FormControl, FormGroup, FormsModule, Validators, ReactiveFormsModule } from '@angular/forms';
import { segUsuarioService } from 'src/model/seg/usuario.service'
import { DialogModule } from 'primeng/dialog';
import { passwordsMatchValidator } from '../util';
import { PasswordModule } from 'primeng/password';
import { ButtonModule } from 'primeng/button';


@Component({
    selector: 'app-publico',
    standalone: true,
    imports: [FormsModule, ReactiveFormsModule, PopoverModule, CheckboxModule, DialogModule, PasswordModule, ButtonModule],
    templateUrl: './publico.component.html',
    styleUrl: './publico.component.scss'
})
export class PublicoComponent {
    showLogin = false;
    formLogin = new FormGroup({
        identificacao: new FormControl<string>('', [Validators.required]),
        senha: new FormControl<string>('', [Validators.required]),
        local: new FormControl<boolean>(true)
    });
    returnUrl: string | undefined;
    primeiroAcesso = false;

    trocaSenhaForm = new FormGroup({
        novaSenha: new FormControl<string>('', [Validators.required, Validators.minLength(6)]),
        confirmacaoSenha: new FormControl<string>('', [Validators.required, Validators.minLength(6)]),
    }, { validators: passwordsMatchValidator('novaSenha', 'confirmacaoSenha') });


    constructor(
        private srv: segUsuarioService,
        private router: Router,
        private route: ActivatedRoute,
    ) { }

    ngOnInit(): void {
        this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/';
        this.srv.PrimeiroAcesso().subscribe({
            next: (data) => {
                this.primeiroAcesso = data;
            }
        });
    }


    login() {
        this.srv.Login(this.formLogin.value).subscribe({
            next: (data) => {
                this.router.navigate([this.returnUrl]);
            }
        });
    }

    ligaSpinner() {
        const blocker = document.getElementById("blocker");
        if (blocker) blocker.style.display = "";
    }

    iniciaLogin(event: any, op: any) {
        op.toggle(event);
    }

    esqueciSenha() {
        const identificacao = this.formLogin.get('identificacao')?.value;
        if (identificacao) {
            this.srv.EsqueciSenha(identificacao).subscribe({
                next: (data) => {
                    this.router.navigate(['/recuperacao-senha']);
                }
            });
        }
    }

    salvarTrocaSenha() {
        const novaSenha = this.trocaSenhaForm.get('novaSenha')?.value;
        if (novaSenha) {
            this.srv.GravaPrimeiraSenha(novaSenha).subscribe({
                next: (data) => {
                    this.primeiroAcesso = false;
                }
            });
        }
    }

}
