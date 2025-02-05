using Microsoft.EntityFrameworkCore.Storage;

namespace api_gerenciar_funcionarios.Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly FuncionarioDbContext _context;
        private IDbContextTransaction? _transaction;

        public UnitOfWork(FuncionarioDbContext context)
        {
            _context = context;
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitAsync()
        {
            if (_transaction != null)
            {
                await _context.SaveChangesAsync();
                await _transaction.CommitAsync();
                _transaction.Dispose();
            }
        }

        public async Task RollbackAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                _transaction.Dispose();
            }
        }
    }

    public interface IUnitOfWork
    {
        Task BeginTransactionAsync();
        Task CommitAsync();
        Task RollbackAsync();
    }
}
