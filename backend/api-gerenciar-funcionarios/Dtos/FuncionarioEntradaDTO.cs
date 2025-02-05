using api_gerenciar_funcionarios.Core.Domain;

namespace api_gerenciar_funcionarios.Dtos
{
    public class FuncionarioEntradaDTO
    {
        public string PrimeiroNome { get; set; } = string.Empty;
        public string Sobrenome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string NumeroDocumento { get; set; } = string.Empty;
        public IEnumerable<string> Telefones { get; set; } = new List<string>();
        public string NomeGestor { get; set; } = string.Empty;
        public DateTime DataNascimento { get; set; }
        public string Senha { get; set; } = string.Empty;
        public Cargo Cargo { get; set; }
        public Guid Id { get; set; } = Guid.Empty;
    }
}
