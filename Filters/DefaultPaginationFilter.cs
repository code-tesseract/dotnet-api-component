using Component.Helpers;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Component.Filters;

public class DefaultPaginationFilter
{
	[FromQuery(Name = "page")]
	[JsonProperty("page")]
	public int Page { get; set; }

	[FromQuery(Name = "per-page")]
	[JsonProperty("per-page")]
	public int PerPage { get; set; }

	[FromQuery(Name = "filters")]
	[JsonProperty("filters")]
	public string? Filters { get; set; }

	[FromQuery(Name = "first-request-time")]
	[JsonProperty("first-request-time")]
	public int FirstRequestTime { get; set; }

	[FromQuery(Name = "sort")]
	[JsonProperty("sort")]
	public string? Sort { get; set; }

	[FromQuery(Name = "is-all")]
	[JsonProperty("is-all")]
	public bool? IsAll { get; set; }

	public DefaultPaginationFilter()
	{
		Page             = 1;
		PerPage          = 10;
		Filters          = null;
		FirstRequestTime = DatetimeHelper.ToUnixTimeSeconds();
		Sort             = null;
		IsAll            = false;
	}

	public DefaultPaginationFilter(int page, int perPage, int? firstRequestTime)
	{
		Page             = page < 1 ? 1 : page;
		PerPage          = perPage > 10 ? 10 : perPage;
		FirstRequestTime = firstRequestTime ?? DatetimeHelper.ToUnixTimeSeconds();
	}
}