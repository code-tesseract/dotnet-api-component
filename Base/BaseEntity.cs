using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Component.Base;

public class BaseEntity
{
    protected BaseEntity()
    {
        var guid = Guid.NewGuid();
        Id = guid.ToString().ToLower();
    }
    
    [Key]
    [StringLength(36)]
    public string Id { get; set; }
    
    [StringLength(36)]
    public string? CreatedBy { get; set; }
    
    [Column(TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }
    
    [StringLength(36)]
    public string? UpdatedBy { get; set; }
    
    [Column(TypeName = "datetime")]
    public DateTime? UpdatedAt { get; set; }
    
    [StringLength(36)]
    public string? DeletedBy { get; set; }
    
    [Column(TypeName = "datetime")]
    public DateTime? DeletedAt { get; set; }
}