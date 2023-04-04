using Component.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Component.Entities;
using Component.Exceptions;
using EntityFramework.Exceptions.SqlServer;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

// ReSharper disable VirtualMemberCallInConstructor

namespace Component.Base;

public class BaseDbContext : DbContext
{
    private readonly BaseDatabaseSetting _dbSetting;

    public BaseDbContext(IOptions<BaseDatabaseSetting> dbSetting)
    {
        _dbSetting = dbSetting.Value;
        ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder opt)
    {
        opt.UseSqlServer(
            $"Data Source={_dbSetting.InstanceName};" +
            $"Initial Catalog={_dbSetting.DatabaseName};" +
            $"Integrated Security={_dbSetting.IntegratedSecurity.ToString()};" +
            $"TrustServerCertificate={_dbSetting.TrustServerCertificate.ToString()};" +
            $"User id={_dbSetting.Username};" +
            $"Password={_dbSetting.Password}"
        );
        opt.UseExceptionProcessor();
    }

    public DbSet<Client> Client { get; set; } = null!;
    public DbSet<ClientWhitelist> ClientWhitelist { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder mb)
    {
        mb.UseCollation(_dbSetting.CollationType.IsNullOrEmpty()
            ? "SQL_Latin1_General_CP1_CI_AS"
            : _dbSetting.CollationType);

        mb.Entity<Client>(e => e.Property(c => c.Status).HasDefaultValueSql($"('{Entities.Client.StatusActive}')"));
        mb.Entity<ClientWhitelist>(e =>
        {
            e.Property(c => c.Status).HasDefaultValueSql($"('{Entities.ClientWhitelist.StatusActive}')");
            e.HasOne(c => c.Client)
                .WithMany(p => p.ClientWhitelists)
                .HasForeignKey(c => c.ClientId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("ForeignKeyClientClientWhitelist");
        });
    }

    public override int SaveChanges()
    {
        ChangeTracker.Entries<BaseEntity>().DatetimeBehavior("CreatedAt", "UpdatedAt");
        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken ct = new())
    {
        ChangeTracker.Entries<BaseEntity>().DatetimeBehavior("CreatedAt", "UpdatedAt");
        return await base.SaveChangesAsync(ct);
    }
}