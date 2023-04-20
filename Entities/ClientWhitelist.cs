using System.ComponentModel.DataAnnotations;
using Component.Base;

// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable PropertyCanBeMadeInitOnly.Global

namespace Component.Entities;

public class ClientWhitelist : BaseEntity
{
    public const string StatusActive = "Active";
    public const string StatusInactive = "Inactive";
    [StringLength(36)] public string ClientId { get; set; } = null!;
    [StringLength(50)] public string Ip { get; set; } = null!;
    [StringLength(50)] public string Status { get; set; } = null!;
    
    public Client Client { get; set; } = null!;
}