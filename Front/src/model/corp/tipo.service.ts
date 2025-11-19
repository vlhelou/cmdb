import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { corpTipo } from 'src/model/corp/tipo'

const httpOptions = {
    headers: new HttpHeaders({ 'Content-Type': 'application/json' })
};

const pathUrl = environment.root +'/api/corp/tipo/';

@Injectable({
    providedIn: 'root'
})
export class TipoService {
    constructor(private http: HttpClient) { }

    ListaAtivos(grupo: string): Observable<corpTipo[]> {
        const url = pathUrl + 'ListaAtivos';
        // url.searchParams.append('grupo', grupo);
        return this.http.get<corpTipo[]>(url, { params: { grupo } });
    }

    Lista(): Observable<corpTipo[]> {
        const url = pathUrl + 'Lista';
        return this.http.get<corpTipo[]>(url);
    }

    Grava(item:any): Observable<corpTipo> {
        const url = pathUrl + 'Grava';
        return this.http.post<corpTipo>(url, item, httpOptions);
    }

    Exclui(id:number): Observable<corpTipo> {
        const url = pathUrl + `Exclui/${id}`;
        return this.http.get<corpTipo>(url);
    }


}
