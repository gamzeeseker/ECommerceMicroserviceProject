namespace StockService.Services.Abstract
{
    public interface IRequestContextService
    {
        Guid UserId { get; }
        string UserName { get; }
    }
}
