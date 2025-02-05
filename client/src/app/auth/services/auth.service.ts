import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, tap } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private apiUrl = 'https://localhost:57752/api/auth'; // Ajuste conforme necessário
  private authStatus = new BehaviorSubject<boolean>(this.isAuthenticated());
  authStatus$ = this.authStatus.asObservable();

  constructor(private http: HttpClient) {}

  login(credentials: { email: string; password: string }): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${this.apiUrl}/login`, credentials).pipe(
      tap((response: LoginResponse) => {
        localStorage.setItem('token', response.token);
        localStorage.setItem('user', JSON.stringify(response.user));
        this.authStatus.next(true);
      })
    );
  }

  logout() {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    this.authStatus.next(false);
  }

  isAuthenticated(): boolean {
    return !!localStorage.getItem('token');
  }

  getToken(): string | null {
    return localStorage.getItem('token');
  }

  getUser(): User | null {
    const user = localStorage.getItem('user');
    return user ? JSON.parse(user) : null;
  }

}

// Interface para o objeto "user"
export interface User {
  id: string; // UUID do usuário
  userName: string; // Nome de usuário
  email: string; // E-mail do usuário
}

// Interface para o retorno completo do login
export interface LoginResponse {
  token: string; // Token JWT
  user: User; // Informações do usuário
}
