import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { corpConfiguracao } from 'src/model/corp/configuracao'

const httpOptions = {
  headers: new HttpHeaders({ 'Content-Type': 'application/json' })
};

const pathUrl = environment.root +'/api/corp/configuracao/';

@Injectable({
  providedIn: 'root'
})
export class ConfiguracaoService {

  constructor(private http: HttpClient) { }

  ArvoreCompleta(): Observable<corpConfiguracao[]> {

    const url = pathUrl + `ArvoreCompleta`;
    console.log('URL',  url);
    return this.http.get<corpConfiguracao[]>(url);
  }

  GravaValor(item: corpConfiguracao): Observable<corpConfiguracao> {
    const url = pathUrl + `GravaValor`;
    return this.http.post<corpConfiguracao>(url, item, httpOptions);
  }

}
