import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { icSegredo } from 'src/model/ic/segredo'

const httpOptions = {
    headers: new HttpHeaders({ 'Content-Type': 'application/json' })
};

const pathUrl = environment.root + '/api/ic/segredo/';
@Injectable({
    providedIn: 'root'
})
export class IcSegredoService {

    constructor(private http: HttpClient) { }

    MeusSegredosPorIc(id: number): Observable<icSegredo[]> {
        const url = pathUrl + `MeusSegredosPorIc/${id}`;
        return this.http.get<icSegredo[]>(url);
    }
    Visualiza(id: number): Observable<any> {
        const url = pathUrl + `Visualiza/${id}`;
        return this.http.get<any>(url);
    }


}
