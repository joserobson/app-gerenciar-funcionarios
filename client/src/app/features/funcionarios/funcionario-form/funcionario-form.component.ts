import { Component, OnInit, inject } from '@angular/core';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, FormArray } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Cargo, Funcionario, FuncionarioService } from '../funcionario.service';
import { AuthService } from '../../../auth/services/auth.service';

@Component({
  selector: 'app-funcionario-form',
  standalone: true,
  templateUrl: './funcionario-form.component.html',
  styleUrls: ['./funcionario-form.component.scss'],
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
})
export class FuncionarioFormComponent implements OnInit {
  funcionarioForm: FormGroup;
  gestores: Funcionario[] = [];
  isEdit = false;
  id: string | null = null;  
  cargos = Object.entries(Cargo) // Converte o enum em uma lista de opções
    .filter(([key]) => !isNaN(Number(key))) // Filtra apenas os valores numéricos
    .map(([key, value]) => ({ id: Number(key), nome: value })); // Formata como objetos

  private fb = inject(FormBuilder);
  private funcionarioService = inject(FuncionarioService);
  private authService = inject(AuthService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);

  constructor() {
    this.funcionarioForm = this.fb.group({
      primeiroNome: ['', Validators.required],
      sobrenome: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      numeroDocumento: ['', Validators.required],
      telefones: this.fb.array([
        ['', [Validators.required, Validators.pattern(/^[0-9]{10,11}$/)]],
        ['', [Validators.required, Validators.pattern(/^[0-9]{10,11}$/)]]
      ]),
      gestorId: [''],
      senha: ['', [Validators.minLength(8), Validators.pattern(/^(?=.*[A-Z])(?=.*\d).+$/)]],
      dataNascimento: ['', [Validators.required, this.validarIdade]],
      cargo: [null, Validators.required],
    });
  }

  ngOnInit() {
    this.id = this.route.snapshot.paramMap.get('id');
    this.carregarGestores();

    if (this.id) {
      this.isEdit = true;
      this.carregarFuncionario();
    }
  }

  carregarGestores() {    
    this.funcionarioService.listar().subscribe({
      next: (data) => this.gestores = data,
      error: (err) => alert('Erro ao carregar gestores: ' + err.error),
    });
  }

  carregarFuncionario() {
    if (this.id) {
      this.funcionarioService.obter(this.id).subscribe({
        next: (data:Funcionario) => {
          this.funcionarioForm.patchValue({
            primeiroNome: data.primeiroNome,
            sobrenome: data.sobrenome,
            email: data.email,
            numeroDocumento: data.numeroDocumento,
            telefones: data.telefones.map(telefone => telefone.toString()),
            gestorId: data.IdGerente,
            dataNascimento: data.dataNascimento,
            cargo: data.cargo
          });
        },
        error: (err) => alert('Erro ao carregar funcionário: ' + err.error),
      });
    }
  }

  salvar() {
    if (this.funcionarioForm.invalid) return;

    const funcionario = {
      ...this.funcionarioForm.value,
      telefones: this.funcionarioForm.value.telefones.map(Number),
      cargo: Number(this.funcionarioForm.value.cargo),
      dataNascimento: new Date(this.funcionarioForm.value.dataNascimento).toISOString(),
    };

    if (this.isEdit && this.id) {
      funcionario.id = this.id;
      this.funcionarioService.atualizar(this.id, funcionario).subscribe({
        next: () => this.router.navigate(['/funcionarios']),
        error: (err) => alert('Erro ao atualizar funcionário: ' + err.error),
      });
    } else {
      this.funcionarioService.criar(funcionario).subscribe({
        next: () => this.router.navigate(['/funcionarios']),
        error: (err) => alert('Erro ao criar funcionário: ' + err.error),
      });
    }
  }

  validarIdade(control: any) {
    const dataNascimento = new Date(control.value);
    const hoje = new Date();
    const idade = hoje.getFullYear() - dataNascimento.getFullYear();
    return idade < 18 ? { menorDeIdade: true } : null;
  }

  get telefones() {
    return this.funcionarioForm.get('telefones') as FormArray;
  }

  adicionarTelefone() {
    if (this.telefones.length >= 5) return;
    
    const novoTelefoneControl = this.fb.control('', [
      Validators.required,
      Validators.pattern(/^[0-9]{10,11}$/)
    ]);
    
    this.telefones.push(novoTelefoneControl);
  }

  removerTelefone(index: number) {
    if (this.telefones.length <= 2) return;
    
    this.telefones.removeAt(index);
  }
}
