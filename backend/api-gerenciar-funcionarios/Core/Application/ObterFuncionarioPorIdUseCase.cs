using api_gerenciar_funcionarios.Core.Domain;
using api_gerenciar_funcionarios.Dtos;
using Microsoft.EntityFrameworkCore;

namespace api_gerenciar_funcionarios.Core.Application
{
    public class ObterFuncionarioPorIdUseCase
    {
        private readonly IFuncionarioRepository _repository;

        public ObterFuncionarioPorIdUseCase(IFuncionarioRepository repository)
        {
            _repository = repository;
        }
        public async Task<UseCaseResult<FuncionarioDTO>> Executar(Guid id)
        {

            var funcionario = await _repository.ObterPorId(id);

            if (funcionario == null)
            {
                return UseCaseResult<FuncionarioDTO>.Fail("Funcionário não encontrado.");
            }

            var dto = new FuncionarioDTO
            {
                Id = funcionario.Id,
                PrimeiroNome = funcionario.Nome,
                Sobrenome = funcionario.Sobrenome,
                Email = funcionario.IdentityUser.Email,
                NumeroDocumento = funcionario.NumeroDocumento,
                Telefones = funcionario.Telefones.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList(),
                NomeGerente = funcionario.NomeGestor,
                DataNascimento = funcionario.DataNascimento.ToString("yyyy-MM-dd"),
                Cargo = funcionario.Cargo
            };

            return UseCaseResult<FuncionarioDTO>.Ok(dto);
        }
    }
}
