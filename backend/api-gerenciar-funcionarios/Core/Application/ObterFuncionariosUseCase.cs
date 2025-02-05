using api_gerenciar_funcionarios.Core.Domain;
using api_gerenciar_funcionarios.Dtos;
using Microsoft.EntityFrameworkCore;

namespace api_gerenciar_funcionarios.Core.Application
{
    public class ObterFuncionariosUseCase
    {
        private readonly IFuncionarioRepository _repository;

        public ObterFuncionariosUseCase(IFuncionarioRepository repository)
        {
            _repository = repository;
        }

        public async Task<UseCaseResult<IEnumerable<FuncionarioDTO>>> Executar()
        {

            var funcionarios = await _repository.ObterTodos();

            var funcionariosDTO = funcionarios.Select(e => new FuncionarioDTO
            {
                Id = e.Id,
                PrimeiroNome = e.Nome,
                Sobrenome = e.Sobrenome,
                Email = e.IdentityUser.Email,
                NumeroDocumento = e.NumeroDocumento,
                Telefones = e.Telefones.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList(),
                NomeGerente = e.NomeGestor,
                DataNascimento = e.DataNascimento.ToString("yyyy-MM-dd"),
                Cargo = e.Cargo,
                UsuarioId = e.IdentityUserId
            }).ToList();

            return UseCaseResult<IEnumerable<FuncionarioDTO>>.Ok(funcionariosDTO);
        }
    }
}
