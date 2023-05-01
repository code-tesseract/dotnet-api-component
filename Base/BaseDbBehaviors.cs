using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

// ReSharper disable InvertIf

namespace Component.Base;

public static class BaseBehaviors
{
	public static string? IdentityId { get; set; }

	public static void DatetimeBehavior(
		this IEnumerable<EntityEntry> entries,
		string?                       createdAtAttribute = null,
		string?                       updatedAtAttribute = null
	)
	{
		foreach (var entry in entries)
		{
			switch (entry.State)
			{
				case EntityState.Added
					when createdAtAttribute != null && entry.Metadata.FindProperty(createdAtAttribute) != null:
					entry.Property(createdAtAttribute).CurrentValue = DateTime.UtcNow.ToLocalTime();
					break;
				case EntityState.Modified
					when updatedAtAttribute != null && entry.Metadata.FindProperty(updatedAtAttribute) != null:
					entry.Property(updatedAtAttribute).CurrentValue = DateTime.UtcNow.ToLocalTime();
					break;
			}
		}
	}

	public static void OwnerBehavior(
		this IEnumerable<EntityEntry> entries,
		string?                       createdByAttribute = null,
		string?                       updatedByAttribute = null
	)
	{
		foreach (var entry in entries)
		{
			switch (entry.State)
			{
				case EntityState.Added
					when createdByAttribute != null && entry.Metadata.FindProperty(createdByAttribute) != null:
					entry.Property(createdByAttribute).CurrentValue = IdentityId;
					break;
				case EntityState.Modified
					when updatedByAttribute != null && entry.Metadata.FindProperty(updatedByAttribute) != null:
					entry.Property(updatedByAttribute).CurrentValue = IdentityId;
					break;
			}
		}
	}

	public static void SoftDeleteBehavior(
		this IEnumerable<EntityEntry> entries,
		string?                       deletedByAttribute = null,
		string?                       deletedAtAttribute = null
	)
	{
		foreach (var entry in entries)
		{
			if (entry.State is EntityState.Deleted)
			{
				entry.State = EntityState.Modified;
				if (deletedByAttribute != null && entry.Metadata.FindProperty(deletedByAttribute) != null)
					entry.Property(deletedByAttribute).CurrentValue = IdentityId;
				if (deletedAtAttribute != null && entry.Metadata.FindProperty(deletedAtAttribute) != null)
					entry.Property(deletedAtAttribute).CurrentValue = DateTime.UtcNow.ToLocalTime();
			}
		}
	}
}