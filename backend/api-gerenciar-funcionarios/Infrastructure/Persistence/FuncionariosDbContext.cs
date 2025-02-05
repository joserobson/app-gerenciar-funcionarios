using api_gerenciar_funcionarios.Core.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace api_gerenciar_funcionarios.Infrastructure.Persistence
{
    public class FuncionarioDbContext : DbContext
    {
        public DbSet<Funcionario> Funcionarios { get; set; }
        public DbSet<IdentityUser> Users { get; set; }
        public FuncionarioDbContext(DbContextOptions<FuncionarioDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Funcionario>()
            .HasKey(f => f.Id); // Define a chave primária            

            // Garantir que o número do documento seja único
            modelBuilder.Entity<Funcionario>()
                .HasIndex(e => e.NumeroDocumento)
                .IsUnique();

            modelBuilder.Entity<Funcionario>()
                 .HasOne(f => f.IdentityUser)
                 .WithOne()
                 .HasForeignKey<Funcionario>(f => f.IdentityUserId);


            // Mapear o enum Cargo como um valor inteiro no banco de dados
            modelBuilder.Entity<Funcionario>()
                .Property(f => f.Cargo)
                .HasConversion<int>();

            // Garantir que o e-mail do usuário seja obrigatório
            modelBuilder.Entity<IdentityUser>()
                .Property(u => u.Email)
                .IsRequired();
           
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(DateTime) || property.ClrType == typeof(DateTime?))
                    {
                        property.SetColumnType("timestamp with time zone");
                    }
                }
            }

        }
    }
}