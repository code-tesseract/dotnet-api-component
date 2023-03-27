using Component.Settings;
using Microsoft.Extensions.Options;

namespace Component.Externals.MediaService;

public class MediaServiceHandler : DelegatingHandler
{
    private readonly MediaSetting _setting;
    public MediaServiceHandler(IOptions<MediaSetting> setting) => _setting = setting.Value;

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
    {
        request.Headers.Add("X-Partner-Id", _setting.MediaClientPartnerId);
        request.Headers.Add("X-Client-Key", _setting.MediaClientKey);
        request.Headers.Add("X-Client-Secret", _setting.MediaClientSecret);

        return await base.SendAsync(request, ct);
    }
}