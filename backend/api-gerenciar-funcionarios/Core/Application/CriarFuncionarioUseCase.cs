using api_gerenciar_funcionarios.Core.Domain;
using api_gerenciar_funcionarios.Dtos;
using api_gerenciar_funcionarios.Infrastructure.Persistence;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace api_gerenciar_funcionarios.Core.Application
{
    public class CriarFuncionarioUseCase
    {
        private readonly IFuncionarioRepository _repository;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IValidator<FuncionarioEntradaDTO> _validator;
        private readonly IUnitOfWork _unitOfWork;

        public CriarFuncionarioUseCase(IFuncionarioRepository repository, UserManager<IdentityUser> userManager, 
            IValidator<FuncionarioEntradaDTO> validator, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _userManager = userManager;
            _validator = validator;
            _unitOfWork = unitOfWork;
        }

        public async Task<UseCaseResult<Funcionario>> Executar(FuncionarioEntradaDTO funcionarioInput)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                var validationResult = await _validator.ValidateAsync(funcionarioInput);
                if (!validationResult.IsValid)
                    return UseCaseResult<Funcionario>.Fail(validationResult.Errors.Select(e => e.ErrorMessage));

                // Verifique se o email já existe
                var existingUser = await _userManager.FindByEmailAsync(funcionarioInput.Email);
                if (existingUser != null)
                {
                    return UseCaseResult<Funcionario>.Fail("Email já em uso.");
                }

                // Verifique se o documento já existe
                var existingFuncionarioComMesmoDocumento = await this._repository.ObterFuncionarioPorNumeroDoDocumento(funcionarioInput.NumeroDocumento);
                if (existingFuncionarioComMesmoDocumento != null)
                {
                    return UseCaseResult<Funcionario>.Fail("Número do Documento já em uso.");
                }

                var user = new IdentityUser
                {
                    UserName = funcionarioInput.Email,
                    Email = funcionarioInput.Email
                };

                var result = await _userManager.CreateAsync(user, funcionarioInput.Senha);
                if (!result.Succeeded)
                    return UseCaseResult<Funcionario>.Fail("Falha ao criar o usuário: " + string.Join(", ", result.Errors.Select(e => e.Description)));

                var funcionario = new Funcionario
                {
                    Nome = funcionarioInput.PrimeiroNome,
                    Sobrenome = funcionarioInput.Sobrenome,
                    NumeroDocumento = funcionarioInput.NumeroDocumento,
                    Telefones = string.Join(',', funcionarioInput.Telefones),
                    NomeGestor = funcionarioInput.NomeGestor,
                    DataNascimento = funcionarioInput.DataNascimento,
                    IdentityUserId = user.Id,
                    Cargo = funcionarioInput.Cargo
                };

                var novoFuncionario = await _repository.Criar(funcionario);

                await _unitOfWork.CommitAsync();
                return UseCaseResult<Funcionario>.Ok(novoFuncionario);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                return UseCaseResult <Funcionario>.Fail($"Erro ao criar funcionário: {ex.Message}");
            }

        }
    }
}
