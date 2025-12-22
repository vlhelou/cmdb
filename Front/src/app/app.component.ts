import { Component, OnInit } from '@angular/core';
import { RouterOutlet, Router } from '@angular/router';
import { ToolbarModule } from 'primeng/toolbar';
import { segUsuarioService } from 'src/model/seg/usuario.service';
import { MenubarModule } from 'primeng/menubar';
import { MenuItem } from 'primeng/api';
import { SplitButtonModule } from 'primeng/splitbutton';
import { DialogModule } from 'primeng/dialog';
import { ButtonModule } from 'primeng/button';
import { PasswordModule } from 'primeng/password';
import { FormControl, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { passwordsMatchValidator } from 'src/app/util';

@Component({
    selector: 'app-root',
    standalone: true,
    imports: [
        RouterOutlet,
        ToolbarModule,
        MenubarModule,
        SplitButtonModule,
        FormsModule,
        ReactiveFormsModule,
        DialogModule,
        PasswordModule,
        ButtonModule,
    ],
    templateUrl: './app.component.html',
    styleUrl: './app.component.scss'
})
export class AppComponent implements OnInit {

    trocaSenhaForm: FormGroup;

    items: MenuItem[] | undefined;

    usuario: any;
    itemsBotao = [
        {
            label: 'Meus dados', routerLink: ['/fileupload']
        },
        {
            label: 'Troca Senha',
            command: () => this.abrirTrocaSenha()
        },
        {
            label: 'Sair',
            command: () => {
                this.sair();
            },
        },
    ];

    mostraTrocaSenha: boolean = false;

    constructor(private srv: segUsuarioService, private router: Router) {
        this.trocaSenhaForm = new FormGroup({
            senhaAtual: new FormControl<string>('', [Validators.required, Validators.minLength(6)]),
            novaSenha: new FormControl<string>('', [Validators.required, Validators.minLength(6)]),
            confirmacaoSenha: new FormControl<string>('', [Validators.required, Validators.minLength(6)]),
        }, { validators: passwordsMatchValidator('novaSenha', 'confirmacaoSenha') });

        this.srv.currentUser.subscribe({
            next: (data) => {
                this.usuario = data;
            }
        });
    }

    ngOnInit(): void {
        this.items = [
            {
                label: 'Home',
                icon: 'pi pi-home',
                routerLink: '/home'
            },
            {
                label: 'Organograma',
                icon: 'pi pi-sitemap',
                routerLink: '/organograma'
            },
            {
                label: 'Usuário',
                icon: 'pi pi-user',
                routerLink: '/usuario/cadastro'
            },
            {
                label: 'Configuração',
                icon: 'pi pi-cog',
                routerLink: '/configuracao'
            },
            {
                label: 'Tipo',
                icon: 'pi pi-list-check',
                routerLink: '/tipo'
            },
            {
                label: 'Embedded',
                icon: 'fa-solid fa-terminal',
                routerLink: '/embedded'
            }
        ]
    }

    sair() {
        this.router.navigate(['/publico']);
        this.srv.Logout();
    }

    abrirTrocaSenha() {
        this.mostraTrocaSenha = true;
        this.trocaSenhaForm.reset();
    }

    salvarTrocaSenha() {
        if (this.trocaSenhaForm.invalid) {
            this.trocaSenhaForm.markAllAsTouched();
            return;
        }
        this.mostraTrocaSenha = false;

        const payload = {
            senhaAtual: this.trocaSenhaForm.value.senhaAtual,
            novaSenha: this.trocaSenhaForm.value.novaSenha,
        };

        this.srv.TrocaSenha(payload).subscribe({
            next: () => {
                this.mostraTrocaSenha = false;
                this.trocaSenhaForm.reset();
            }
        });
    }

}
