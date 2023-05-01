using Component.Exceptions;
using Component.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Component.Externals.MediaService;

public class MediaServiceHandler : DelegatingHandler
{
	private readonly MediaServiceSetting _mediaSetting;
	public MediaServiceHandler(IOptions<MediaServiceSetting> setting) => _mediaSetting = setting.Value;

	protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
	{
		if (_mediaSetting is { Enable: false })
			throw new HttpException(StatusCodes.Status503ServiceUnavailable,
									"Sorry, the media service is currently unavailable. Please check your media service configuration " +
									"and make sure the configuration already has the `Enable` property set to true.");

		request.Headers.Add("X-Partner-Id", _mediaSetting.PartnerId);
		request.Headers.Add("X-Client-Key", _mediaSetting.ClientKey);
		request.Headers.Add("X-Client-Secret", _mediaSetting.ClientSecret);

		return await base.SendAsync(request, ct);
	}
}