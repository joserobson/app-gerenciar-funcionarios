using api_gerenciar_funcionarios.Core.Domain;

namespace api_gerenciar_funcionarios.Dtos
{
    public class FuncionarioDTO
    {
        public Guid Id { get; set; }
        public string PrimeiroNome { get; set; } = string.Empty;
        public string Sobrenome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string NumeroDocumento { get; set; } = string.Empty;
        public IEnumerable<string> Telefones { get; set; } = [];
        public string NomeGerente { get; set; } = string.Empty;
        public string IdGerente { get; set; } = string.Empty;
        public string DataNascimento { get; set; }
        public Cargo Cargo { get; set; } = Cargo.Funcionario;
        public string? UsuarioId { get; internal set; }
    }
}
