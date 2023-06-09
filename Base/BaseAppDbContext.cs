using Component.Entities;
using Microsoft.EntityFrameworkCore;

namespace Component.Base;

public class BaseAppDbContext : BaseDbContext
{
	public DbSet<Client>          Client          { get; set; } = null!;
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

		mb.Entity<Client>(e => e.Property(c => c.Status).HasDefaultValue(Entities.Client.StatusActive));
		mb.Entity<ClientWhitelist>(e =>
		{
			e.Property(c => c.Status).HasDefaultValue(Entities.ClientWhitelist.StatusActive);
			e.HasOne(c => c.Client).WithMany(p => p.ClientWhitelists).HasForeignKey(c => c.ClientId)
			 .OnDelete(DeleteBehavior.Restrict);
		});
	}
}