using Common.Request;
using StockService.Models;

namespace StockService.Services.Abstract
{
    public interface IProductService
    {
        IResponse<bool> Create(ProductModel model);
        IResponse<ProductModel> Get(Guid id);
        IResponse<List<ProductModel>> List();
    }
}
