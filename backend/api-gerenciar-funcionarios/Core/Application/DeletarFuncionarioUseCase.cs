using api_gerenciar_funcionarios.Core.Domain;

namespace api_gerenciar_funcionarios.Core.Application
{
    public class DeletarFuncionarioUseCase
    {
        private readonly IFuncionarioRepository _repository;

        public DeletarFuncionarioUseCase(IFuncionarioRepository repository)
        {
            _repository = repository;
        }

        public async Task<UseCaseResult<bool>> Executar(Guid id)
        {
            var funcionarioExistente = await _repository.ObterPorId(id);
            if (funcionarioExistente == null)
            {
                return UseCaseResult<bool>.Fail("Funcionário não encontrado.");
            }

            await _repository.Deletar(id);
            return UseCaseResult<bool>.Ok(true);
        }
    }
}
