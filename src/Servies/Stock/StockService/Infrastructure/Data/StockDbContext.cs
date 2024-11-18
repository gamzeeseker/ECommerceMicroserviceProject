using Microsoft.EntityFrameworkCore;
using StockService.Domain.Entity;
using StockService.Services.Abstract;


public class StockDbContext : DbContext
{
    private readonly IRequestContextService _requestContext;

    public DbSet<StockEntity> Stocks { get; set; }

    public DbSet<ProductEntity> Products { get; set; }

    public StockDbContext(DbContextOptions<StockDbContext> options, IRequestContextService requestContext) : base(options)
    {
        this._requestContext = requestContext;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<StockEntity>().HasQueryFilter(s => !s.IsDeleted);
        modelBuilder.Entity<ProductEntity>().HasQueryFilter(s => !s.IsDeleted);

        modelBuilder.Entity<StockEntity>()
            .HasOne(s => s.ProductEntity)
            .WithMany(p => p.Stocks)
            .HasForeignKey(s => s.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    public override int SaveChanges()
    {
        SetAuditInformation();
        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        SetAuditInformation();
        return await base.SaveChangesAsync(cancellationToken);
    }

    private void SetAuditInformation()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.Entity is BaseEntity &&
                        (e.State == EntityState.Added || e.State == EntityState.Modified));

        foreach (var entry in entries)
        {
            var entity = (BaseEntity)entry.Entity;
            if (entry.State == EntityState.Added)
            {
                entity.CreataDate = DateTime.UtcNow;
                entity.CreateUser = _requestContext.UserId;
            }

            entity.UpdateDate = DateTime.UtcNow;
            entity.UpdateUser = _requestContext.UserId;
        }
    }
}