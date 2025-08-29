import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment'
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

const httpOptions = {
    headers: new HttpHeaders({ 'Content-Type': 'application/json' })
};

const pathUrl = environment.root + '/api/seg/organograma/';


@Injectable({
    providedIn: 'root'
})
export class OrganogramaService {
    constructor(private http: HttpClient) { }

    Pesquisa(prm: any): Observable<any[]> {
        const url = pathUrl + `Pesquisa`;
        return this.http.post<any[]>(url, prm, httpOptions);
    }



}
