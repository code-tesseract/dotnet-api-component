using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

// ReSharper disable InvertIf

namespace Component.Base;

public static class BaseBehaviors
{
    private static string? _identityOwnerId;
    public static void SetOwnerIdentityOwnerId(string? identityOwnerId) => _identityOwnerId = identityOwnerId;

    public static void DatetimeBehavior(
        this IEnumerable<EntityEntry> entries,
        string? createdAtAttribute = null,
        string? updatedAtAttribute = null
    )
    {
        foreach (var entry in entries)
        {
            switch (entry.State)
            {
                case EntityState.Added
                    when createdAtAttribute != null && entry.Metadata.FindProperty(createdAtAttribute) != null:
                    entry.Property(createdAtAttribute).CurrentValue = DateTime.UtcNow;
                    break;
                case EntityState.Modified
                    when updatedAtAttribute != null && entry.Metadata.FindProperty(updatedAtAttribute) != null:
                    entry.Property(updatedAtAttribute).CurrentValue = DateTime.UtcNow;
                    break;
            }
        }
    }

    public static void OwnerBehavior(
        this IEnumerable<EntityEntry> entries,
        string? createdByAttribute = null,
        string? updatedByAttribute = null
    )
    {
        foreach (var entry in entries)
        {
            switch (entry.State)
            {
                case EntityState.Added
                    when createdByAttribute != null && entry.Metadata.FindProperty(createdByAttribute) != null:
                    entry.Property(createdByAttribute).CurrentValue = _identityOwnerId;
                    break;
                case EntityState.Modified
                    when updatedByAttribute != null && entry.Metadata.FindProperty(updatedByAttribute) != null:
                    entry.Property(updatedByAttribute).CurrentValue = _identityOwnerId;
                    break;
            }
        }
    }

    public static void SoftDeleteBehavior(
        this IEnumerable<EntityEntry> entries,
        string? deletedByAttribute = null,
        string? deletedAtAttribute = null
    )
    {
        foreach (var entry in entries)
        {
            if (entry.State is EntityState.Deleted)
            {
                if (deletedByAttribute != null && entry.Metadata.FindProperty(deletedByAttribute) != null)
                    entry.Property(deletedByAttribute).CurrentValue = _identityOwnerId;
                if (deletedAtAttribute != null && entry.Metadata.FindProperty(deletedAtAttribute) != null)
                    entry.Property(deletedAtAttribute).CurrentValue = DateTime.UtcNow;
                entry.State = EntityState.Modified;
            }
        }
    }
}