import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment'
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { segOrganograma } from 'src/model/seg/organograma'

const httpOptions = {
    headers: new HttpHeaders({ 'Content-Type': 'application/json' })
};

const pathUrl = environment.root + '/api/seg/organograma/';


@Injectable({
    providedIn: 'root'
})
export class OrganogramaService {
    constructor(private http: HttpClient) { }

    Pesquisa(prm: any): Observable<segOrganograma[]> {
        const url = pathUrl + `Pesquisa`;
        return this.http.post<segOrganograma[]>(url, prm, httpOptions);
    }

    ListaCompleta(): Observable<segOrganograma> {
        const url = pathUrl + `ListaCompleta`;
        return this.http.get<segOrganograma>(url);
    }

    Grava(prm: any): Observable<segOrganograma[]> {
        const url = pathUrl + `Grava`;
        return this.http.post<segOrganograma[]>(url, prm, httpOptions);
    }

    MudaPaternidade(id: number, idnovopai: number): Observable<segOrganograma> {
        const url = pathUrl + `MudaPaternidade/${id}/${idnovopai}`;
        return this.http.get<segOrganograma>(url);
    }



}
