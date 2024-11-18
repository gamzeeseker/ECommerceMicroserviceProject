using Common.Request;
using StockService.Domain.Entity;
using StockService.Domain.Interface;
using StockService.Models;
using StockService.Services.Abstract;

namespace StockService.Services.Concrete
{
    public class StockService : IStockService
    {
        private readonly IUnitOfWork _unitOfWork;

        public StockService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IResponse<StockEntity>> GetStockAsync(Guid productId)
        {
            var product = (await _unitOfWork.StockRepository.FindAsync(c => c.ProductId == productId)).FirstOrDefault();

            if (product is null)
                return Response<StockEntity>.Fail(product);
            else
                return Response<StockEntity>.Success(product);
        }

        public async Task<IResponse<bool>> UpdateStockAsync(StockModel stockModel)
        {
            var product = await _unitOfWork.StockRepository.GetByIdAsync(stockModel.ProductId);
            if (!(product is null))
            {
                product.Quantity = stockModel.Quantity;
                _unitOfWork.StockRepository.Update(product);
            }
            return Response<bool>.Success(true);
        }
    }
}
