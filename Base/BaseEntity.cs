using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable SwitchStatementMissingSomeEnumCasesNoDefault
// ReSharper disable MemberCanBePrivate.Global

namespace Component.Base;

public class BaseEntity
{
    protected BaseEntity()
    {
        var guid = Guid.NewGuid();
        Id = guid.ToString().ToLower();
    }
    
    [Key]
    [Column(Order = 0)]
    [StringLength(36)]
    public string Id { get; set; }

    [Column(TypeName = "datetime", Order = 99)]
    public DateTime CreatedAt { get; set; }

    [Column(TypeName = "datetime", Order = 100)]
    public DateTime? UpdatedAt { get; set; }
}