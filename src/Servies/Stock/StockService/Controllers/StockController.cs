using Common.Request;
using Microsoft.AspNetCore.Mvc;
using StockService.Domain.Entity;
using StockService.Models;
using StockService.Services.Abstract;

namespace StockService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StockController : ControllerBase
    {
        private readonly IStockService _stockService;

        public StockController(IStockService stockService)
        {
            this._stockService = stockService;
        }
        [HttpPost]
        public async Task<IResponse<bool>> PostAsync(StockModel model) =>
            await _stockService.UpdateStockAsync(model);

        [HttpGet]
        public async Task<IResponse<StockEntity>> GetAsync(Guid productId) =>
            await _stockService.GetStockAsync(productId);
    }
}
