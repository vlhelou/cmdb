import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment'
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';
import { segUsuario } from './usuario'
import { map } from 'rxjs/operators';
import { segOrganograma } from 'src/model/seg/organograma'


const httpOptions = {
    headers: new HttpHeaders({ 'Content-Type': 'application/json' })
};

const pathUrl = environment.root + '/api/seg/usuario/';

@Injectable({
    providedIn: 'root'
})
export class segUsuarioService {
    private currentUserSubject: BehaviorSubject<any>;
    public currentUser: Observable<any>;

    constructor(private http: HttpClient) {
        const strLocal = localStorage.getItem('currentUser');
        if (strLocal) {
            this.currentUserSubject = new BehaviorSubject<any>(JSON.parse(strLocal));
        } else {
            this.currentUserSubject = new BehaviorSubject<any>(null);
        }
        this.currentUser = this.currentUserSubject.asObservable();
    }

    public get currentUserValue(): segUsuario {
        return this.currentUserSubject.value;
    }

    public usuarioAtual() {
        return this.currentUserSubject.value;
    }

    Login(credencial: any): Observable<any> {
        const url = pathUrl + 'login';
        return this.http.post<any>(url, credencial, httpOptions)
            .pipe(map(user => {
                if (user && user.token) {
                    localStorage.setItem('currentUser', JSON.stringify(user));
                    this.currentUserSubject.next(user);
                    return user;
                }
            }));
    }

    Logout() {
        localStorage.removeItem('currentUser');
        this.currentUserSubject.next(null);
    }

    MeusOrganogramas(): Observable<segOrganograma[]> {
        const url = pathUrl + `MeusOrganogramas`;
        return this.http.get<segOrganograma[]>(url);
    }


    Lista(): Observable<segUsuario[]> {
        const url = pathUrl + `Lista`;
        return this.http.get<segUsuario[]>(url);
    }


    Exclui(id: number): Observable<void> {
        const url = pathUrl + `Exclui/${id}`;
        return this.http.get<void>(url);
    }

    Grava(usuario: any): Observable<segUsuario> {
        const url = pathUrl + `Grava`;
        return this.http.post<segUsuario>(url, usuario, httpOptions);
    }

    TrocaSenha(prm: any): Observable<void> {
        const url = pathUrl + `TrocaSenha`;
        return this.http.post<void>(url, prm, httpOptions);
    }

    RecuperaSenhaChave(prm: any): Observable<void> {
        const url = pathUrl + `RecuperaSenhaChave`;
        return this.http.post<void>(url, prm, httpOptions);
    }

    EsqueciSenha(identificacao: string): Observable<void> {
        const url = pathUrl + `EsqueciSenha/${identificacao}`;
        return this.http.get<void>(url);
    }

    Pesquisa(prm: any): Observable<segUsuario[]> {
        const url = pathUrl + `Pesquisa`;
        return this.http.post<segUsuario[]>(url, prm, httpOptions);
    }


    Eu(): Observable<segUsuario> {
        const url = pathUrl + `Eu`;
        return this.http.get<segUsuario>(url);
    }

    AlteraEmail(email:string): Observable<segUsuario> {
        const prm={email};
        const url = pathUrl + `AlteraEmail`;
        return this.http.post<segUsuario>(url, prm, httpOptions);
    }

    PrimeiroAcesso(): Observable<boolean> {
        const url = pathUrl + `PrimeiroAcesso`;
        return this.http.get<boolean>(url);
    }


}
