import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { icIc } from 'src/model/ic/ic';


const httpOptions = {
  headers: new HttpHeaders({ 'Content-Type': 'application/json' })
};

const pathUrl = environment.root + '/api/ic/ic/';

@Injectable({
  providedIn: 'root'
})
export class IcService {

  constructor(private http: HttpClient) { }

  BuscaComFamilia(id: number): Observable<icIc> {
    const url = pathUrl + `BuscaComFamilia/${id}`;
    return this.http.get<icIc>(url);
  }

  ResumoPorIC(id: number): Observable<any[]> {
    const url = pathUrl + `ResumoPorIC/${id}`;
    return this.http.get<any[]>(url);
  }

  Exclui(id: number): Observable<void> {
    const url = pathUrl + `Exclui/${id}`;
    return this.http.get<void>(url);
  }

  Grava(item: icIc): Observable<icIc> {
    const url = pathUrl + `Grava`;
    return this.http.post<icIc>(url, item, httpOptions);
  }

  Pesquisa(prm: any): Observable<icIc[]> {
    const url = pathUrl + `Pesquisa`;
    console.log(url)
    return this.http.post<icIc[]>(url, prm, httpOptions);
  }

  MudaPaternidade(id: number, idnovopai: number): Observable<icIc> {
    const url = pathUrl + `MudaPaternidade/${id}/${idnovopai}`;
    return this.http.get<icIc>(url);
  }


}
