using Microsoft.EntityFrameworkCore;

namespace Component.Base;

public class BaseDbContext : DbContext
{
    protected BaseDbContext(DbContextOptions options) : base(options)
    {
    }

    public BaseDbContext(DbContextOptions<BaseDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder mb)
    {
        mb.Entity<BaseEntity>().HasQueryFilter(b => b.IsDeleted == false);
        mb.Ignore<BaseEntity>();
    }

    public override int SaveChanges()
    {
        ChangeTracker.Entries<BaseEntity>().DatetimeBehavior("CreatedAt", "UpdatedAt");
        ChangeTracker.Entries<BaseEntity>().OwnerBehavior("CreatedBy", "UpdatedBy");
        ChangeTracker.Entries<BaseEntity>().SoftDeleteBehavior("IsDeleted", "DeletedBy", "DeletedAt");
        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken ct = new())
    {
        ChangeTracker.Entries<BaseEntity>().DatetimeBehavior("CreatedAt", "UpdatedAt");
        ChangeTracker.Entries<BaseEntity>().OwnerBehavior("CreatedBy", "UpdatedBy");
        ChangeTracker.Entries<BaseEntity>().SoftDeleteBehavior("IsDeleted", "DeletedBy", "DeletedAt");
        return await base.SaveChangesAsync(ct);
    }
}