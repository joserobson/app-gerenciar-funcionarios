using Microsoft.AspNetCore.Identity;

namespace api_gerenciar_funcionarios.Config
{    
    public class CustomIdentityErrorDescriber : IdentityErrorDescriber
    {
        public override IdentityError DefaultError()
        {
            return new IdentityError { Code = nameof(DefaultError), Description = "Ocorreu um erro desconhecido." };
        }

        public override IdentityError PasswordTooShort(int length)
        {
            return new IdentityError { Code = nameof(PasswordTooShort), Description = $"A senha deve ter pelo menos {length} caracteres." };
        }

        public override IdentityError PasswordRequiresNonAlphanumeric()
        {
            return new IdentityError { Code = nameof(PasswordRequiresNonAlphanumeric), Description = "A senha deve conter pelo menos um caractere especial." };
        }

        public override IdentityError PasswordRequiresDigit()
        {
            return new IdentityError { Code = nameof(PasswordRequiresDigit), Description = "A senha deve conter pelo menos um número (0-9)." };
        }

        public override IdentityError PasswordRequiresLower()
        {
            return new IdentityError { Code = nameof(PasswordRequiresLower), Description = "A senha deve conter pelo menos uma letra minúscula (a-z)." };
        }

        public override IdentityError PasswordRequiresUpper()
        {
            return new IdentityError { Code = nameof(PasswordRequiresUpper), Description = "A senha deve conter pelo menos uma letra maiúscula (A-Z)." };
        }
    }
}
