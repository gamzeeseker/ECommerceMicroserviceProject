using StockService.Domain.Entity;

namespace StockService.Services
{
    public class StartupDate
    {
        public static void InitialData(IServiceProvider serviceProvider)
        {

            using (var scoup = serviceProvider.CreateScope())
            {

                using (var context = scoup.ServiceProvider.GetRequiredService<StockDbContext>())
                {
                    if (!context.Database.CanConnect())
                    {
                        context.Database.EnsureCreated();
                    }

                    var product = new ProductEntity
                    {
                        Name = "Sample Product",
                        Description = "This is a sample product.",
                        Price = 29.99m,
                        Stocks = new List<StockEntity>
                        {
                            new StockEntity { Quantity = 10 },
                            new StockEntity { Quantity = 20 }
                        }
                    };

                    context.Products.Add(product);
                    context.SaveChanges();
                }
            }
        }
    }
}
