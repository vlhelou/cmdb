import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { OverlayPanelModule } from 'primeng/overlaypanel';
import { FormControl, FormGroup, FormsModule, Validators, ReactiveFormsModule } from '@angular/forms';
import { SegUsuarioService } from 'src/model/seg/usuario.service'


@Component({
    selector: 'app-publico',
    standalone: true,
    imports: [OverlayPanelModule, FormsModule, ReactiveFormsModule],
    templateUrl: './publico.component.html',
    styleUrl: './publico.component.scss'
})
export class PublicoComponent {

    formLogin = new FormGroup({
        email: new FormControl<string>('adm@cmdb.com.br', [Validators.required]),
        senha: new FormControl<string>('123456', [Validators.required]),
    });
    returnUrl: string | undefined;


    constructor(
        private srv: SegUsuarioService,
        private router: Router,
        private route: ActivatedRoute,
    ) { }

    ngOnInit(): void {
        this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/';
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
}
