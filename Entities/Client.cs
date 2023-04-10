using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Component.Base;
using Microsoft.EntityFrameworkCore;

namespace Component.Entities;

[Table("Client")]
[Index(nameof(Key), Name = "UniqueClientKey", IsUnique = true)]
[Index(nameof(Type), Name = "IndexClientType")]
[Index(nameof(Status), Name = "IndexClientStatus")]
[Index(nameof(CreatedAt), Name = "IndexClientCreatedAt")]
public class Client : BaseEntity
{
    public const string TypeDevice = "Device";
    public const string TypeService = "Service";
    
    public const string StatusActive = "Active";
    public const string StatusInactive = "Inactive";

    public Client()
    {
        ClientWhitelists = new HashSet<ClientWhitelist>();
    }
    
    [Column(Order = 1)]
    [StringLength(255)]
    public string Key { get; set; } = null!;
    
    [Column(Order = 2)]
    [StringLength(255)]
    public string Secret { get; set; } = null!;
    
    [Column(Order = 3)] 
    [StringLength(50)] 
    public string? Grant { get; set; }
    
    [Column(Order = 4)] 
    [StringLength(50)] 
    public string Type { get; set; } = null!;
    
    [Column(TypeName = "datetime", Order = 5)]
    public DateTime? ExpiryOn { get; set; }
    
    [Column(Order = 6)]
    [StringLength(50)]
    public string Status { get; set; } = null!;
    
    [InverseProperty("Client")]
    public ICollection<ClientWhitelist>? ClientWhitelists { get; set; }
}