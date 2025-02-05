import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class FuncionarioService {
  private apiUrl = 'https://localhost:57752/api/funcionario';

  constructor(private http: HttpClient) {}

  listar(): Observable<Funcionario[]> {
    return this.http.get<Funcionario[]>(this.apiUrl);
  }

  obter(id: string): Observable<Funcionario> {
    return this.http.get<Funcionario>(`${this.apiUrl}/${id}`);
  }

  criar(funcionario: any): Observable<any> {
    return this.http.post(this.apiUrl, funcionario);
  }

  atualizar(id: string, funcionario: any): Observable<any> {
    return this.http.put(`${this.apiUrl}/${id}`, funcionario);
  }

  deletar(id: string): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`);
  }
}

export interface Funcionario {
  id: string; // UUID
  primeiroNome: string;
  sobrenome: string;
  email: string;
  numeroDocumento: string;
  telefones: string[]; // Array de strings
  nomeGerente: string;
  IdGerente: string;
  dataNascimento: string; // Pode ser ajustado para `Date` se necessário
  cargo: Cargo;
  usuarioId: string;
}

export enum Cargo {
  Funcionario = 1,
  Lider = 2,
  Diretor = 3,
}
