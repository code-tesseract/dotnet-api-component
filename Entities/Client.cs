using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Component.Base;
using Microsoft.EntityFrameworkCore;

namespace Component.Entities;

[Index(nameof(Key), IsUnique = true)]
[Index(nameof(Type))]
[Index(nameof(Status))]
public class Client : BaseEntity
{
    public const string TypeDevice  = "Device";
    public const string TypeService = "Service";

    public const string StatusActive   = "Active";
    public const string StatusInactive = "Inactive";

    [StringLength(255)]             public string    Key      { get; set; } = null!;
    [StringLength(255)]             public string    Secret   { get; set; } = null!;
    [StringLength(50)]              public string?   Grant    { get; set; }
    [StringLength(50)]              public string    Type     { get; set; } = null!;
    [Column(TypeName = "datetime")] public DateTime? ExpiryOn { get; set; }
    [StringLength(50)]              public string    Status   { get; set; } = null!;

    public ICollection<ClientWhitelist>? ClientWhitelists { get; set; }
}