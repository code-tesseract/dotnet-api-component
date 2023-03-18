using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

// ReSharper disable UnusedAutoPropertyAccessor.Local
// ReSharper disable SwitchStatementMissingSomeEnumCasesNoDefault

namespace Component.Base;

public static class BaseBehaviors
{
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
}