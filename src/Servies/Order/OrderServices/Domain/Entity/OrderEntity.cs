namespace OrderServices.Domain.Entity
{
    public class OrderEntity : BaseEntity
    {
        public Guid OrderUserId { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}