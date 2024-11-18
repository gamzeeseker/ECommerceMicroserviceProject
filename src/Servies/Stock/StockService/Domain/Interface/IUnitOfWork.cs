using StockService.Domain.Entity;

namespace StockService.Domain.Interface
{

    public interface IUnitOfWork
    {
        IRepository<StockEntity> StockRepository { get; }
        IRepository<ProductEntity> ProductRepository { get; }
        Task<int> CommitAsync();
    }
}
