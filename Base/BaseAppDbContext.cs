using Component.Entities;
using Microsoft.EntityFrameworkCore;

namespace Component.Base;

public class BaseAppDbContext : BaseDbContext
{
    public DbSet<Client> Client { get; set; } = null!;
    public DbSet<ClientWhitelist> ClientWhitelist { get; set; } = null!;

    protected BaseAppDbContext(DbContextOptions options) : base(options)
    {
    }

    public BaseAppDbContext(DbContextOptions<BaseAppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder mb)
    {
        base.OnModelCreating(mb);

        mb.Entity<Client>(e =>
        {
            e.Property(c => c.Status).HasDefaultValue(Entities.Client.StatusActive);

            e.HasKey(p => p.Id).HasName(SetKeyName("Client", "Id"));
            e.HasIndex(p => p.Key, SetIndexName("Client", "Key", true)).IsUnique();
            e.HasIndex(p => p.Type, SetIndexName("Client", "Type"));
            e.HasIndex(p => p.Status, SetIndexName("Client", "Status"));
            e.HasIndex(p => p.CreatedBy, SetIndexName("Client", "CreatedBy"));
            e.HasIndex(p => p.CreatedAt, SetIndexName("Client", "CreatedAt")).IsDescending();
            e.HasIndex(p => p.UpdatedBy, SetIndexName("Client", "UpdatedBy"));
            e.HasIndex(p => p.UpdatedAt, SetIndexName("Client", "UpdatedAt")).IsDescending();
            e.HasIndex(p => p.DeletedBy, SetIndexName("Client", "DeletedBy"));
            e.HasIndex(p => p.DeletedAt, SetIndexName("Client", "DeletedAt")).IsDescending();
        });

        mb.Entity<ClientWhitelist>(e =>
        {
            e.Property(c => c.Status).HasDefaultValue(Entities.ClientWhitelist.StatusActive);
            
            e.HasOne(c => c.Client)
                .WithMany(p => p.ClientWhitelists)
                .HasForeignKey(c => c.ClientId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName(SetConstraintName("ClientWhitelist", "ClientId", "Client", "Id"));

            e.HasKey(p => p.Id).HasName(SetKeyName("ClientWhitelist", "Id"));
            e.HasIndex(new[] { "ClientId", "Ip" },
                SetIndexName("ClientWhitelist", new[] { "ClientId", "Ip" }, true)).IsUnique();
            e.HasIndex(p => p.Status, SetIndexName("ClientWhitelist", "Status"));
            e.HasIndex(p => p.CreatedBy, SetIndexName("ClientWhitelist", "CreatedBy"));
            e.HasIndex(p => p.CreatedAt, SetIndexName("ClientWhitelist", "CreatedAt")).IsDescending();
            e.HasIndex(p => p.UpdatedBy, SetIndexName("ClientWhitelist", "UpdatedBy"));
            e.HasIndex(p => p.UpdatedAt, SetIndexName("ClientWhitelist", "UpdatedAt")).IsDescending();
            e.HasIndex(p => p.DeletedBy, SetIndexName("ClientWhitelist", "DeletedBy"));
            e.HasIndex(p => p.DeletedAt, SetIndexName("ClientWhitelist", "DeletedAt")).IsDescending();
        });
    }
}