using StockService.Domain.Entity;
using StockService.Domain.Interface;
using StockService.Infrastructure.Repository;

namespace StockService.Infrastructure.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly StockDbContext _context;

        public UnitOfWork(StockDbContext context)
        {
            _context = context;
            StockRepository = new Repository<StockEntity>(_context);
            ProductRepository = new Repository<ProductEntity>(_context);
        }

        public IRepository<StockEntity> StockRepository { get; }

        public IRepository<ProductEntity> ProductRepository { get; }

        public async Task<int> CommitAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
