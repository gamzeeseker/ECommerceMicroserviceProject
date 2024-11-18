using Common.Request;
using StockService.Models;
using StockService.Services.Abstract;

namespace StockService.Services.Concrete
{
    public class ProductService : IProductService
    {
        IResponse<bool> IProductService.Create(ProductModel model)
        {
            throw new NotImplementedException();
        }

        IResponse<ProductModel> IProductService.Get(Guid id)
        {
            throw new NotImplementedException();
        }

        IResponse<List<ProductModel>> IProductService.List()
        {
            throw new NotImplementedException();
        }
    }
}
