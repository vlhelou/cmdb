import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { IcDependencia } from 'src/model/ic/dependencia';


const httpOptions = {
  headers: new HttpHeaders({ 'Content-Type': 'application/json' })
};

const pathUrl = environment.root + '/api/ic/dependencia/';

@Injectable({
  providedIn: 'root'
})
export class DependenciaService {
  constructor(private http: HttpClient) { }


  Grava(item: any): Observable<any> {
    const url = pathUrl + `Grava`;
    return this.http.post<any>(url, item, httpOptions);
  }


  DependenciasPorIC(id: number, dependente:boolean): Observable<IcDependencia[]> {
    const url = pathUrl + `DependenciasPorIC/${id}/${dependente}`;
    return this.http.get<IcDependencia[]>(url);
  }

  Exclui(id: number): Observable<IcDependencia[]> {
    const url = pathUrl + `Exclui/${id}`;
    return this.http.get<IcDependencia[]>(url);
  }


}
