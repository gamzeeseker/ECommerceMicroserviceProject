using Common.Request;
using OrderServices.Models;

namespace OrderServices.Services.Abstract
{
    public interface IOrderService
    {
        Task<IResponse<List<OrderListModel>>> OrdersAsync(Guid id);
        Task<IResponse<bool>> CreateOrderAsync(CreateOrderModel model);
    }
}
