namespace OrderServices.Models
{
    public class CreateOrderModel
    {
        public Guid UserId { get; set; }
        public Guid ProductId { get; set; }
        public int Qunatity { get; set; }
    }

    public class OrderListModel
    {
        public Guid Id { get; set; }
        public Guid OrderUserId { get; set; }
        public Guid ProductId { get; set; }
        public int Qunatity { get; set; }
    }
}
