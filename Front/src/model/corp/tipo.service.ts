import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import {corpTipo} from 'src/model/corp/tipo'

const httpOptions = {
  headers: new HttpHeaders({ 'Content-Type': 'application/json' })
};

const pathUrl =  '/api/corp/tipo/';

@Injectable({
  providedIn: 'root'
})
export class TipoService {
  constructor(private http: HttpClient) { }

    ListaAtivos(grupo:string): Observable<corpTipo[]> {
      const url = new URL(pathUrl + 'ListaAtivos', environment.root);
      url.searchParams.append('grupo', grupo);
      console.log(url.toString());
      return this.http.get<corpTipo[]>(url.toString());
    }
  

}
