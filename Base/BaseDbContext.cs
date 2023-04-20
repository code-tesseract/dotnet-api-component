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

    protected static string SetKeyName(string table, string column) => $"PK_{table}__{column}";
    protected static string SetConstraintName(string table, string column, string refTable, string refColumn)
        => $"FK_{table}__{column}_{refTable}__{refColumn}";
    protected static string SetIndexName(string table, object column, bool unique = false)
    {
        var indexPrefix = unique ? "UQ" : "IX";
        if (!string.IsNullOrEmpty(table) && table.Length > 1)
            table = $"{table[0].ToString().ToUpper() + table[1..].ToLower()}";
        else table = $"{table[0].ToString().ToUpper()}";

        string columnString;
        if (column is string[] columns) columnString = string.Join('-', columns);
        else columnString = (string)column;

        return $"{indexPrefix}_{table}__{columnString}";
    }
}