using Common.Request;
using StockService.Domain.Entity;
using StockService.Models;

namespace StockService.Services.Abstract
{
    public interface IStockService
    {
        Task<IResponse<bool>> UpdateStockAsync(StockModel stockModel);

        Task<IResponse<StockEntity>> GetStockAsync(Guid productId);
    }
}
