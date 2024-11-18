using Common.Request;
using Microsoft.AspNetCore.Mvc;
using OrderServices.Models;
using OrderServices.Services.Abstract;

namespace OrderServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : BaseController
    {
        private readonly IOrderService orderService;

        public OrderController(IOrderService orderService)
        {
            this.orderService = orderService;
        }

        [HttpPost]
        public async Task<IResponse<bool>> Create(CreateOrderModel model)
            => await orderService.CreateOrderAsync(model);
    }
}
