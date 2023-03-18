using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Component.Base;
using Microsoft.EntityFrameworkCore;

// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable PropertyCanBeMadeInitOnly.Global

namespace Component.Entities;

[Table("ClientWhitelist")]
[Index(nameof(ClientId), nameof(Ip), Name = "UniqueClientWhitelistClientIdIp", IsUnique = true)]
[Index(nameof(Status), Name = "IndexClientWhitelistStatus")]
[Index(nameof(CreatedAt), Name = "IndexClientWhitelistCreatedAt")]
public class ClientWhitelist : BaseEntity
{
    public const string StatusActive = "Active";
    public const string StatusInactive = "Inactive";
    
    [Column(Order = 1)]
    [StringLength(36)]
    public string ClientId { get; set; } = null!;
    
    [Column(Order = 2)]
    [StringLength(50)]
    public string Ip { get; set; } = null!;
    
    [Column(Order = 3)]
    [StringLength(50)]
    public string Status { get; set; } = null!;
    
    [ForeignKey(nameof(ClientId))]
    [InverseProperty("ClientWhitelists")]
    public Client Client { get; set; } = null!;
}