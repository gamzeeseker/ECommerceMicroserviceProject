using OrderServices.Domain.Entity;
using OrderServices.Domain.Interface;
using OrderServices.Infrastructure.Repository;

namespace OrderServices.Infrastructure.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly OrderDbContext _context;

        public UnitOfWork(OrderDbContext context)
        {
            _context = context;
            OrderRepository = new Repository<OrderEntity>(_context);
        }

        public IRepository<OrderEntity> OrderRepository { get; }

        public async Task<int> CommitAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
