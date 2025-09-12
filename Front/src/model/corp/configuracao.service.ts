import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { corpConfiguracao } from 'src/model/corp/configuracao'

const httpOptions = {
  headers: new HttpHeaders({ 'Content-Type': 'application/json' })
};

const pathUrl = '/api/corp/configuracao/';

@Injectable({
  providedIn: 'root'
})
export class ConfiguracaoService {

  constructor(private http: HttpClient) { }

  ArvoreCompleta(): Observable<corpConfiguracao[]> {
    const url = new URL(pathUrl + 'ArvoreCompleta', environment.root);
    return this.http.get<corpConfiguracao[]>(url.toString());
  }

  GravaValor(item: corpConfiguracao): Observable<corpConfiguracao> {
    const url = new URL(pathUrl + 'GravaValor', environment.root); 
    return this.http.post<corpConfiguracao>(url.toString(), item, httpOptions);
  }

}
