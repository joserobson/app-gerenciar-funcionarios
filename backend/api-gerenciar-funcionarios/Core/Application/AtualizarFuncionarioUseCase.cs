using api_gerenciar_funcionarios.Core.Domain;
using api_gerenciar_funcionarios.Dtos;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace api_gerenciar_funcionarios.Core.Application
{
    public class AtualizarFuncionarioUseCase
    {
        private readonly IFuncionarioRepository _repository;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IValidator<FuncionarioEntradaDTO> _validator;

        public AtualizarFuncionarioUseCase(IFuncionarioRepository repository, UserManager<IdentityUser> userManager, IValidator<FuncionarioEntradaDTO> validator)
        {
            _repository = repository;
            _userManager = userManager;
            _validator = validator;
        }

        public async Task<UseCaseResult<Funcionario>> Executar(FuncionarioEntradaDTO funcionarioInput)
        {

            var validationResult = await _validator.ValidateAsync(funcionarioInput);
            if (!validationResult.IsValid)
                return UseCaseResult<Funcionario>.Fail(validationResult.Errors.Select(e => e.ErrorMessage));

            var funcionarioExistente = await _repository.ObterPorId(funcionarioInput.Id);
            if (funcionarioExistente == null)
            {
                return UseCaseResult<Funcionario>.Fail("Funcionário não encontrado.");
            }

            // Verifique se o email já existe
            var existingUser = await _userManager.FindByEmailAsync(funcionarioInput.Email);
            if (existingUser != null && funcionarioExistente.IdentityUserId != existingUser.Id)
            {
                return UseCaseResult<Funcionario>.Fail("Email já em uso.");
            }

            // Verifique se o documento já existe
            var existingFuncionarioComMesmoDocumento = await this._repository.ObterFuncionarioPorNumeroDoDocumento(funcionarioInput.NumeroDocumento);
            if (existingFuncionarioComMesmoDocumento != null && funcionarioExistente.Id != existingFuncionarioComMesmoDocumento.Id)
            {
                return UseCaseResult<Funcionario>.Fail("Número do Documento já em uso.");
            }

            funcionarioExistente.Nome = funcionarioInput.PrimeiroNome;
            funcionarioExistente.Sobrenome = funcionarioInput.Sobrenome;
            funcionarioExistente.NumeroDocumento = funcionarioInput.NumeroDocumento;
            funcionarioExistente.Telefones = string.Join(',', funcionarioInput.Telefones);
            funcionarioExistente.NomeGestor = funcionarioInput.NomeGestor;
            funcionarioExistente.DataNascimento = funcionarioInput.DataNascimento;
            funcionarioExistente.Cargo = funcionarioInput.Cargo;
            funcionarioExistente.IdentityUser.Email = funcionarioInput.Email;
            

            await _repository.Atualizar(funcionarioExistente);
            return UseCaseResult<Funcionario>.Ok(funcionarioExistente);
        }
    }
}
