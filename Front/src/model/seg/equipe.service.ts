import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment'
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { segEquipe } from 'src/model/seg/equipe'

const httpOptions = {
    headers: new HttpHeaders({ 'Content-Type': 'application/json' })
};

const pathUrl = environment.root + '/api/seg/equipe/';
@Injectable({
    providedIn: 'root',
})
export class EquipeService {

    constructor(private http: HttpClient) { }

    UsuarioPorOrganograma(idIc: number): Observable<segEquipe[]> {
        const url = pathUrl + `UsuarioPorOrganograma/${idIc}`;
        return this.http.get<segEquipe[]>(url);
    }

    OrganogramasPorUsuario(idusuario: number): Observable<segEquipe[]> {
        const url = pathUrl + `OrganogramasPorUsuario/${idusuario}`;
        return this.http.get<segEquipe[]>(url);
    }

    Adicionar(idusuario: number, idOrg: number): Observable<any> {
        const url = pathUrl + `Adicionar/${idusuario}/${idOrg}`;
        return this.http.get<any>(url);
    }


    Exclui(id: number): Observable<void> {
        const url = pathUrl + `Exclui/${id}`;
        return this.http.get<void>(url);
    }

    
    ExcluiMeuOrganograma(id: number): Observable<void> {
        const url = pathUrl + `ExcluiMeuOrganograma/${id}`;
        return this.http.get<void>(url);
    }

}
