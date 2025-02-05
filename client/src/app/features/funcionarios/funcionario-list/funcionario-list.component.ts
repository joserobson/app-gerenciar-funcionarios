import { Component, OnInit, inject } from '@angular/core';
import { RouterModule, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { Cargo, FuncionarioService } from '../funcionario.service';

@Component({
  selector: 'app-funcionario-list',
  standalone: true,
  templateUrl: './funcionario-list.component.html',
  styleUrls: ['./funcionario-list.component.scss'],
  imports: [CommonModule, RouterModule],
})
export class FuncionarioListComponent implements OnInit {
  funcionarios: any[] = [];
  private funcionarioService = inject(FuncionarioService);
  private router = inject(Router);

  ngOnInit() {
    this.carregarFuncionarios();
  }

  carregarFuncionarios() {
    this.funcionarioService.listar().subscribe({
      next: (data) => (this.funcionarios = data.map((funcionario) => ({ ...funcionario, cargo: funcionario.cargo as Cargo, }))),
      error: (err) => alert('Erro ao carregar funcionários: ' + err.error),
    });
  }

  getCargoLabel(cargo: Cargo): string {
    switch (cargo) {
      case Cargo.Funcionario:
        return 'Funcionário';
      case Cargo.Lider:
        return 'Líder';
      case Cargo.Diretor:
        return 'Diretor';
      default:
        return 'Desconhecido';
    }
  }


  deletarFuncionario(id: string) {
    if (confirm('Tem certeza que deseja excluir este funcionário?')) {
      this.funcionarioService.deletar(id).subscribe({
        next: () => this.carregarFuncionarios(),
        error: (err) => alert('Erro ao excluir funcionário: ' + err.error),
      });
    }
  }
}
