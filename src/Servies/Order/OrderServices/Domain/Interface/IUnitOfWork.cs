using OrderServices.Domain.Entity;

namespace OrderServices.Domain.Interface
{

    public interface IUnitOfWork
    {
        IRepository<OrderEntity> OrderRepository { get; }

        Task<int> CommitAsync();
    }


}
