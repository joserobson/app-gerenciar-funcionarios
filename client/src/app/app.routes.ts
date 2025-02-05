import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthGuard } from './auth/auth.guard';
import { LoginComponent } from './auth/page/login/login.component';
import { FuncionarioListComponent } from './features/funcionarios/funcionario-list/funcionario-list.component';
import { FuncionarioFormComponent } from './features/funcionarios/funcionario-form/funcionario-form.component';

export const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' }, // Redireciona para 'login' ao acessar a rota raiz ('/')
  { path: 'login', component: LoginComponent },
  { path: 'funcionarios', component: FuncionarioListComponent, canActivate: [AuthGuard] },
  { path: 'funcionarios/novo', component: FuncionarioFormComponent, canActivate: [AuthGuard] },
  { path: 'funcionarios/editar/:id', component: FuncionarioFormComponent, canActivate: [AuthGuard] },
  { path: '**', redirectTo: 'login' }, // Redireciona para 'login' em caso de rota inválida
];