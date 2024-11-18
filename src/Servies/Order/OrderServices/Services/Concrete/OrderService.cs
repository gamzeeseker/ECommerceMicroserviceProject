using Common.Events;
using Common.Request;
using EventBus.Factory.EventBus.Factory;
using Newtonsoft.Json;
using OrderServices.Domain.Interface;
using OrderServices.Models;
using OrderServices.Services.Abstract;

namespace OrderServices.Services.Concrete
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<OrderService> _logger;
        private readonly IServiceProvider _serviceProvider;

        public OrderService(IUnitOfWork unitOfWork,
            IHttpClientFactory httpClientFactory, ILogger<OrderService> logger,
            IServiceProvider serviceProvider)
        {
            _unitOfWork = unitOfWork;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public async Task<IResponse<bool>> CreateOrderAsync(CreateOrderModel model)
        {
            var client = _httpClientFactory.CreateClient("ProductService");

            _logger.LogInformation("start calling product service");
            var response = await client.GetAsync("https://localhost:44333/api/Stock?productId=" + model.ProductId);

            var responseContent = string.Empty;

            if (response.IsSuccessStatusCode)
            {
                responseContent = await response.Content.ReadAsStringAsync();
            }

            _logger.LogInformation("product service response", responseContent);

            var quantity = JsonConvert.DeserializeObject<Response<Common.Models.StockModel>>(responseContent);

            if (quantity.Data.Quantity - model.Qunatity < 0)
                return Response<bool>.Fail(false, "Stock count error");

            var eventBus =
                EventBusFactory.Create(new EventBus.Base.EventBusConfig()
                {
                    EventBusConnectionString = "http:\\localhost:5672",
                    SubscriberClientAppName = "order",
                }, _serviceProvider);

            eventBus.Publish(new NotifySmsEvent()
            {
                NotifyModel = new Common.Models.SmsNotifyModel()
                {

                    Addresses = new List<string> { "5555555555", "New Order" }
                }
            });

            eventBus.Publish(new NotifyEmailEvent()
            {
                NotifyModel = new Common.Models.EmailNotifyModel()
                {
                    Addresses = new List<string> { "test@test.com" },
                    Body = "your order was success",
                    Subject = "order"
                }
            });

            return Response<bool>.Success(true);
        }

        public async Task<IResponse<List<OrderListModel>>> OrdersAsync(Guid id)
        {
            var data = await _unitOfWork.OrderRepository.GetAllAsync();
            return Response<List<OrderListModel>>.Success(data.Select(c => new OrderListModel()
            {
                Id = c.Id,
                ProductId = c.ProductId,
                Qunatity = c.Quantity,
                OrderUserId = c.OrderUserId
            }).ToList());
        }
    }
}
