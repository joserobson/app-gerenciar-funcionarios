using api_gerenciar_funcionarios.Dtos;
using FluentValidation;

namespace api_gerenciar_funcionarios.Validators
{
    public class FuncionarioValidator : AbstractValidator<FuncionarioEntradaDTO>
    {
        public FuncionarioValidator()
        {
            RuleFor(x => x.PrimeiroNome)
                .NotEmpty().WithMessage("Campo Nome obrigatório.")
                .MinimumLength(2).WithMessage("O Nome deve ter pelo menos 2 caracteres.");

            RuleFor(x => x.Sobrenome)
                .NotEmpty().WithMessage("Campo Sobrenome obrigatório.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Campo Email obrigatório.")
                .EmailAddress().WithMessage("Formato de email inválido.");

            RuleFor(x => x.NumeroDocumento)
                .NotEmpty().WithMessage("Campo Numero do Documento obrigatório.");

            RuleFor(x => x.Telefones)
                .NotEmpty().WithMessage("Campo Telefone obrigatório.")
                .Must(t => t.Count() >= 2).WithMessage("Forneça pelo menos 2 Telefones.");

            RuleFor(x => x.DataNascimento)
                .NotEmpty().WithMessage("Campo Data de Nascimento obrigatório.")
                .Must(data => CalcularIdade(data) >= 18)
                .WithMessage("O funcionário não pode ser menor de idade.");

            RuleFor(x => x.Senha)
                .NotEmpty().WithMessage("Campo Senha obrigatório.")
                .MinimumLength(8).WithMessage("A senha deve ter pelo menos 8 caracteres.")
                .Matches("[A-Z]").WithMessage("A senha deve conter pelo menos uma letra maiúscula (A-Z).")
                .Matches("[0-9]").WithMessage("A senha deve conter pelo menos um número (0-9).")
                .Matches("[^a-zA-Z0-9]").WithMessage("A senha deve conter pelo menos um caractere especial.")
                .When(x => string.IsNullOrEmpty(x.Id.ToString()) || x.Id == Guid.Empty); // Só se for um novo usuário

            RuleFor(x => x.Cargo)
                .IsInEnum().WithMessage("Cargo inválido.");
        }

        private int CalcularIdade(DateTime dataNascimento)
        {
            var hoje = DateTime.Today;
            var idade = hoje.Year - dataNascimento.Year;
            if (dataNascimento.Date > hoje.AddYears(-idade)) idade--;
            return idade;
        }
    }
}
