using System.Text;
using Component.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ContentDispositionHeaderValue = System.Net.Http.Headers.ContentDispositionHeaderValue;

namespace Component.Externals.MediaService;

public class MediaService : IMediaService
{
    private readonly HttpClient _client;
    public MediaService(IHttpClientFactory factory) => _client = factory.CreateClient("MediaServiceClient");

    public async Task<Response> Requirements(CancellationToken ct)
    {
        var (method, path) = MediaServiceEndpoints.Requirements;
        var requestMessage = new HttpRequestMessage(method, path);
        using var responseMessage = await _client.SendAsync(requestMessage, ct);
        var responseBodyString = await responseMessage.Content.ReadAsStringAsync(ct);
        return JsonConvert.DeserializeObject<Response>(responseBodyString)!;
    }

    public async Task<Response> UploadAsync(IFormFile? file, CancellationToken ct)
    {
        using var formData = new MultipartFormDataContent();
        if (file != null)
            formData.Add(new StreamContent(file.OpenReadStream()), "File", file.FileName);
        else
        {
            var nullContent = new ByteArrayContent(Array.Empty<byte>());
            nullContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
            {
                Name = "File",
                FileName = "null",
                Size = 0
            };
            formData.Add(nullContent);
        }

        var (method, path) = MediaServiceEndpoints.Upload;
        var requestMessage = new HttpRequestMessage(method, path);
        requestMessage.Content = formData;

        using var responseMessage = await _client.SendAsync(requestMessage, ct);
        var responseBodyString = await responseMessage.Content.ReadAsStringAsync(ct);
        return JsonConvert.DeserializeObject<Response>(responseBodyString)!;
    }

    public async Task<Response> UploadBase64Async(string? base64Content, CancellationToken ct)
    {
        var (method, path) = MediaServiceEndpoints.UploadBase64;
        var requestMessage = new HttpRequestMessage(method, path);
        requestMessage.Content = new StringContent(
            JsonConvert.SerializeObject(new { Base64Content = base64Content?? string.Empty }),
            Encoding.UTF8,
            "application/json"
        );

        using var responseMessage = await _client.SendAsync(requestMessage, ct);
        var responseBodyString = await responseMessage.Content.ReadAsStringAsync(ct);
        return JsonConvert.DeserializeObject<Response>(responseBodyString)!;
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