using Component.Enums;
using Component.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;

namespace Component.Models;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global
public class Response
{
	public string? Name        { get; set; }
	public string  Message     { get; set; }
	public int     Code        { get; set; }
	public int     Status      { get; set; }
	public double? RequestTime { get; set; }
	public object? Data        { get; set; }

	[JsonProperty("Errors", NullValueHandling = NullValueHandling.Ignore)]
	public object? Errors { get; set; }

	public object? Meta { get; set; }

	public Response(
		object? data        = null,
		string? message     = null,
		string? name        = null,
		int     status      = StatusCodes.Status200OK,
		int     code        = (int)ResponseCodeEnum.Success,
		double? requestTime = null,
		object? errors      = null,
		object? meta        = null
	)
	{
		Name        = name ?? (status is 200 ? "Success" : ReasonPhrases.GetReasonPhrase(status));
		Message     = message ?? ResponseMessageEnum.Success.GetDescription();
		Code        = code;
		Status      = status;
		RequestTime = requestTime;
		Data        = data ?? new object();
		Errors      = errors;
		Meta        = meta ?? new object();
	}
}