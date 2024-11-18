namespace StockService.Domain.Entity
{
    public class ProductEntity : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string? Code { get; set; }
        public string? Color { get; set; }
        public decimal? Height { get; set; }
        public decimal? Width { get; set; }
        public decimal? Length { get; set; }
        public ICollection<StockEntity> Stocks { get; set; } = new List<StockEntity>();

        public static ProductEntity Create(string name, string code, string color, decimal height, decimal width, decimal length)
        {
            return new ProductEntity
            {
                Name = name,
                Code = code,
                Color = color,
                Length = length,
                Height = height,
                Width = width
            };
        }
    }
}