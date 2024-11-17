import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment'
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, BehaviorSubject } from 'rxjs';
import { segUsuario } from './usuario'
import { map } from 'rxjs/operators';
import { Nullable } from 'primeng/ts-helpers';


const httpOptions = {
    headers: new HttpHeaders({ 'Content-Type': 'application/json' })
};

const pathUrl = environment.root + '/api/seg/usuario/';

@Injectable({
    providedIn: 'root'
})
export class SegUsuarioService {
    private currentUserSubject: BehaviorSubject<Nullable<segUsuario>> ;
    currentUser: Observable<Nullable<segUsuario>>;

    constructor(private http: HttpClient) {
        this.currentUserSubject= new BehaviorSubject(JSON.parse(localStorage.getItem('user')!));
        this.currentUser = this.currentUserSubject.asObservable();
    }

    public get currentUserValue(): segUsuario|Nullable {
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

}
