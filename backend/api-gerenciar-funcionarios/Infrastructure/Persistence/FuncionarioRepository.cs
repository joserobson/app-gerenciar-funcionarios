using api_gerenciar_funcionarios.Core.Domain;
using api_gerenciar_funcionarios.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;


namespace api_gerenciar_funcionarios.Infrastructure.Repositories
{
    public class FuncionarioRepository : IFuncionarioRepository
    {
        private readonly FuncionarioDbContext _context;

        public FuncionarioRepository(FuncionarioDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Funcionario>> ObterTodos()
        {
            return await _context
                .Funcionarios
                .Include(f => f.IdentityUser)
                .ToListAsync();
        }

        public async Task<Funcionario?> ObterPorId(Guid id)
        {
            return await _context.Funcionarios                
                .Include(f => f.IdentityUser)
                .FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task<Funcionario> Criar(Funcionario funcionario)
        {
            _context.Funcionarios.Add(funcionario);
            await _context.SaveChangesAsync();
            return funcionario;
        }

        public async Task Atualizar(Funcionario funcionario)
        {
            _context.Funcionarios.Update(funcionario);
            await _context.SaveChangesAsync();
        }

        public async Task Deletar(Guid id)
        {
            var funcionario = await _context.Funcionarios.FindAsync(id);
            if (funcionario != null)
            {
                _context.Funcionarios.Remove(funcionario);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Funcionario?> ObterFuncionarioPorNumeroDoDocumento(string numeroDoDocumento)
        {
            return await _context.Funcionarios.FirstOrDefaultAsync(f => f.NumeroDocumento == numeroDoDocumento);
        }
    }
}
