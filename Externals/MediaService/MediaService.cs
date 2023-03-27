using Component.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Component.Externals.MediaService;

public class MediaService : IMediaService
{
    private readonly HttpClient _client;
    public MediaService(IHttpClientFactory factory) => _client = factory.CreateClient("MediaServiceClient");

    public async Task<Response> Requirements(CancellationToken ct)
    {
        var (method, path) = MediaServiceEndpoints.Requirements;
        var requestMessage = new HttpRequestMessage(method, path);
        var responseMessage = await _client.SendAsync(requestMessage, ct);
        var responseBodyString = await responseMessage.Content.ReadAsStringAsync(ct);
        return JsonConvert.DeserializeObject<Response>(responseBodyString)!;
    }

    public Task<Response> UploadAsync(IFormFile file, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<Response> UploadBase64Async(string base64Content, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<FileStreamResult> PreviewAsync(Guid mediaId, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<FileStreamResult> PreviewThumbnailAsync(Guid mediaId, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}