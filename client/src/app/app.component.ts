import { Component, inject, OnInit } from '@angular/core';
import { Router, RouterOutlet } from '@angular/router';
import { AuthService } from './auth/services/auth.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet,CommonModule],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'  
})
export class AppComponent implements OnInit{
  isAuthenticated = false;
  userName: string | null = null;
  private authService = inject(AuthService);
  private router = inject(Router);

  ngOnInit() {
    this.isAuthenticated = this.authService.isAuthenticated();
    this.userName = <string>this.authService.getUser()?.userName;

    // Monitora mudanças de login/logout
    this.authService.authStatus$.subscribe((status) => {
      this.isAuthenticated = status;
      this.userName = <string>this.authService.getUser()?.userName;
    });
  }

  logout() {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}
