using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Component.Base;

[Index(nameof(CreatedBy), nameof(UpdatedBy), nameof(DeletedBy))]
[Index(nameof(CreatedAt), nameof(UpdatedAt), nameof(DeletedAt), IsDescending = new[] { true, true, true })]
public class BaseEntity
{
	protected BaseEntity()
	{
		var guid = Guid.NewGuid();
		Id = guid.ToString().ToLower();
	}

	[Key] [StringLength(36)]        public string    Id        { get; set; }
	[StringLength(36)]              public string?   CreatedBy { get; set; }
	[Column(TypeName = "datetime")] public DateTime  CreatedAt { get; set; }
	[StringLength(36)]              public string?   UpdatedBy { get; set; }
	[Column(TypeName = "datetime")] public DateTime? UpdatedAt { get; set; }
	[StringLength(36)]              public string?   DeletedBy { get; set; }
	[Column(TypeName = "datetime")] public DateTime? DeletedAt { get; set; }
}