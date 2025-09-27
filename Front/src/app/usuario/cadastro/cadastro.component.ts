import { Component, OnInit, signal } from '@angular/core';
import { TableModule } from 'primeng/table';
import { segUsuarioService } from 'src/model/seg/usuario.service'
import { segUsuario } from 'src/model/seg/usuario'
import { NgClass } from '@angular/common';
import { FormGroup, Validators, FormControl, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ConfirmPopupModule } from 'primeng/confirmpopup';
import { ConfirmationService } from 'primeng/api';

@Component({
    selector: 'app-cadastro',
    imports: [TableModule, FormsModule, ReactiveFormsModule, NgClass, ConfirmPopupModule],
    templateUrl: './cadastro.component.html',
    styleUrl: './cadastro.component.scss',
    providers: [ConfirmationService]
})
export class UsuarioCadastroComponent implements OnInit {

    listaUsuarios = signal<segUsuario[]>([]);
    showFormulario = signal<boolean>(false);
    nomeHeader = signal<string>('Novo Usuário');

    formulario = new FormGroup({
        id: new FormControl<number>(0),
        identificacao: new FormControl<string>('', [Validators.required]),
        administrador: new FormControl<boolean>(false),
        ativo: new FormControl<boolean>(true),
        local: new FormControl<boolean>(true),
        email: new FormControl<string>('', [Validators.required, Validators.email]),
    });


    constructor(private srv: segUsuarioService, private confirmationService: ConfirmationService) { }

    ngOnInit(): void {
        this.showFormulario.set(false);
        this.srv.Lista().subscribe(usuarios => {
            this.listaUsuarios.set(usuarios);
        });
    }

    edita(item: segUsuario) {
        this.showFormulario.set(true);
        this.nomeHeader.set(item.identificacao);
        this.formulario.reset({
            id: item.id,
            identificacao: item.identificacao,
            email: item.email,
            ativo: item.ativo,
            local: item.local,
            administrador: item.administrador,
        });
    }

    novoUsuario() {
        this.formulario.reset({
            id: 0,
            identificacao: '',
            email: '',
            ativo: true,
            local: true,
            administrador: false,
        });
        this.showFormulario.set(true);
        this.nomeHeader.set('Novo Usuário');
    }

    salvar() {
        this.srv.Grava(this.formulario.value ).subscribe({
            next: data => {
                this.showFormulario.set(false);
                this.srv.Lista().subscribe(usuarios => {
                    this.listaUsuarios.set(usuarios);
                });
            },
            error: err => {
                console.error('Erro ao gravar usuário', err);
            }
        });
    }

    excluir(item: segUsuario, event: Event) {
         this.confirmationService.confirm({
            target: event.currentTarget as EventTarget,
            message: `confirma a exclusão do usuário ${item.identificacao}?`,
            icon: 'pi pi-exclamation-triangle',
            acceptLabel: 'Excluir',
            rejectLabel: 'Cancelar',
            rejectButtonProps: {
                severity: 'info',
            },
            acceptButtonProps: {
                severity: 'danger',
            },
            accept: () => {
                this.srv.Exclui(item.id).subscribe({
                    next: () => {
                        this.srv.Lista().subscribe(usuarios => {
                            this.listaUsuarios.set(usuarios);
                        });
                    }
                });
            },
            reject: () => {
            }
        });
    }



}
