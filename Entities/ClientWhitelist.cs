using System.ComponentModel.DataAnnotations;
using Component.Base;
using Microsoft.EntityFrameworkCore;

// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable PropertyCanBeMadeInitOnly.Global

namespace Component.Entities;

[Index(nameof(ClientId), nameof(Ip), IsUnique = true)]
[Index(nameof(Status))]
public class ClientWhitelist : BaseEntity
{
	public const              string StatusActive   = "Active";
	public const              string StatusInactive = "Inactive";
	[StringLength(36)] public string ClientId { get; set; } = null!;
	[StringLength(50)] public string Ip       { get; set; } = null!;
	[StringLength(50)] public string Status   { get; set; } = null!;

	public Client Client { get; set; } = null!;
}