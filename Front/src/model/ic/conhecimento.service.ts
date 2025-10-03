import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { IcConhecimento } from 'src/model/ic/conhecimento';

const httpOptions = {
  headers: new HttpHeaders({ 'Content-Type': 'application/json' })
};

const pathUrl = environment.root + '/api/ic/conhecimento/';

@Injectable({
  providedIn: 'root'
})
export class IcConhecimentoService {
    constructor(private http: HttpClient) { }


    Grava(item: any): Observable<IcConhecimento> {
        const url = pathUrl + `Grava`;
        return this.http.post<IcConhecimento>(url, item, httpOptions);
    }

    ConhecimentosPorIC(id:number): Observable<IcConhecimento[]> {
        const url = pathUrl + `ConhecimentosPorIC/${id}`;
        return this.http.get<IcConhecimento[]>(url);
    }

    Exclui(id:number): Observable<IcConhecimento> {
        const url = pathUrl + `Exclui/${id}`;
        return this.http.get<IcConhecimento>(url);
    }


}
