using Common.Request;
using Microsoft.AspNetCore.Mvc;
using StockService.Models;
using StockService.Services.Abstract;

namespace StockService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            this._productService = productService;
        }

        [HttpGet("{id}")]
        public IResponse<ProductModel> Get(Guid id)
            => _productService.Get(id);

        [HttpGet]
        public IResponse<List<ProductModel>> Get()
            => _productService.List();

        [HttpPost]
        public IResponse<bool> Create(ProductModel model) =>
            _productService.Create(model);
    }
}
