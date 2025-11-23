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
  
  
}
