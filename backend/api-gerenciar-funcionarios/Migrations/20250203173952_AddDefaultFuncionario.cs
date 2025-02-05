using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.AspNetCore.Identity;
using System;
using api_gerenciar_funcionarios.Core.Domain;

namespace api_gerenciar_funcionarios.Migrations
{
    public partial class AddDefaultFuncionario : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var hasher = new PasswordHasher<IdentityUser>();

            // Gerar um GUID para o usuário (IdentityUser)
            var identityUserId = Guid.NewGuid().ToString();

            var identityUser = new IdentityUser
            {
                Id = identityUserId,
                UserName = "ADMIN",
                Email = "ADMIN@COMPANY.COM",
                NormalizedUserName = "ADMIN",
                NormalizedEmail = "ADMIN@COMPANY.COM",
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString(),
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                PhoneNumber = "11987654321",
                PhoneNumberConfirmed = true,
                TwoFactorEnabled = false,
                LockoutEnabled = false,
                AccessFailedCount = 0
            };

            identityUser.PasswordHash = hasher.HashPassword(identityUser, "SenhaForte123");

            // Inserir o IdentityUser na tabela "Users" (AspNetUsers)
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[]
                {
                    "Id", "UserName", "NormalizedUserName", "Email", "NormalizedEmail",
                    "PasswordHash", "EmailConfirmed", "SecurityStamp", "ConcurrencyStamp",
                    "PhoneNumber", "PhoneNumberConfirmed", "TwoFactorEnabled",
                    "LockoutEnd", "LockoutEnabled", "AccessFailedCount"
                },
                values: new object[]
                {
                    identityUser.Id, identityUser.UserName, identityUser.NormalizedUserName,
                    identityUser.Email, identityUser.NormalizedEmail, identityUser.PasswordHash,
                    identityUser.EmailConfirmed, identityUser.SecurityStamp, identityUser.ConcurrencyStamp,
                    identityUser.PhoneNumber, identityUser.PhoneNumberConfirmed, identityUser.TwoFactorEnabled,
                    null, identityUser.LockoutEnabled, identityUser.AccessFailedCount
                }
            );

            // Inserir o Funcionario
            migrationBuilder.InsertData(
                table: "Funcionarios",
                columns: new[]
                {
                    "Id", "Nome", "Sobrenome", "NumeroDocumento", "Telefones",
                    "NomeGestor", "DataNascimento", "Cargo", "IdentityUserId"
                },
                values: new object[]
                {
                    Guid.NewGuid(), "José", "Assis", "123456789", "1532999934312,1535999934312",
                    "Gestor Exemplo", DateTime.UtcNow.AddYears(-30), Cargo.Diretor, identityUser.Id
                }
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM Funcionarios WHERE Nome = 'José' AND Sobrenome = 'Assis'");
            migrationBuilder.Sql("DELETE FROM Users WHERE UserName = 'admin'");
        }
    }
}
