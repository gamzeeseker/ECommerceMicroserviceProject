using Microsoft.EntityFrameworkCore;
using OrderServices.Domain.Entity;
using OrderServices.Services;

public class OrderDbContext : DbContext
{
    private readonly IRequestContextService _requestContext;

    public DbSet<OrderEntity> Orders { get; set; }

    public OrderDbContext(DbContextOptions<OrderDbContext> options, IRequestContextService requestContext) : base(options)
    {
        this._requestContext = requestContext;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<OrderEntity>().HasQueryFilter(s => !s.IsDeleted);
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