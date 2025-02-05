namespace api_gerenciar_funcionarios.Core.Domain
{
    public interface IFuncionarioRepository
    {
        Task<IEnumerable<Funcionario>> ObterTodos();
        Task<Funcionario> ObterPorId(Guid id);
        Task<Funcionario> Criar(Funcionario funcionario);
        Task Atualizar(Funcionario funcionario);
        Task Deletar(Guid id);

        Task<Funcionario?> ObterFuncionarioPorNumeroDoDocumento(string numeroDoDocumento);
    }
}
