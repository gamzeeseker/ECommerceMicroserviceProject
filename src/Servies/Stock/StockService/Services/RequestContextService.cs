using StockService.Services.Abstract;

namespace StockService.Services
{
    public class RequestContextService : IRequestContextService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RequestContextService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string UserName => _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "System";
        public Guid UserId => Guid.Parse("37ae1dd3-f6e8-4c7f-a47d-3314dd36d961");
    }
}
