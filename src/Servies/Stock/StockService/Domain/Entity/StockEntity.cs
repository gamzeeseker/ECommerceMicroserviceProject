using System.ComponentModel.DataAnnotations.Schema;

namespace StockService.Domain.Entity
{
    public class StockEntity : BaseEntity
    {
        public int Quantity { get; set; }
        public Guid ProductId { get; set; }

        [ForeignKey(nameof(ProductId))]
        public ProductEntity ProductEntity { get; set; }

    }
}
