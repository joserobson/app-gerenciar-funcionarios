using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace api_gerenciar_funcionarios.Core.Domain
{
    public class Funcionario
    {

        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(100)]
        public string Nome { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Sobrenome { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string NumeroDocumento { get; set; } = string.Empty;

        [Required]
        [MinLength(10)] // Pelo menos dois telefones
        public string Telefones { get; set; } = string.Empty;

        public string NomeGestor { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        public DateTime DataNascimento { get; set; }

        [Required]
        public Cargo Cargo { get; set; }

        // Relacionamento com o IdentityUser
        public string? IdentityUserId { get; set; }
        public IdentityUser? IdentityUser { get; set; }
    }
}
