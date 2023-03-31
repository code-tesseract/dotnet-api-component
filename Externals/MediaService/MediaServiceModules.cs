using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using Component.Exceptions;
using Component.Externals.MediaService.Utilities;
using Component.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Component.Externals.MediaService;

public interface IMediaServiceModules
{
    Task<Response> Requirements(CancellationToken ct);
    Task<Response> UploadAsync(IFormFile? file, CancellationToken ct);
    Task<Response> UploadsAsync(IEnumerable<IFormFile>? files, CancellationToken ct);
    Task<Response> UploadBase64Async(string? base64Content, CancellationToken ct);
    Task<(Stream stream, string contentType)> PreviewAsync(string id, CancellationToken ct);
    Task<(Stream stream, string contentType)> PreviewThumbnailAsync(string id, CancellationToken ct);
    Task<(Stream stream, string fileName)> DownloadAsync(string id, CancellationToken ct);
    Task<(Stream stream, string fileName)> DownloadThumbAsync(string id, CancellationToken ct);
}

public class MediaServiceModules : IMediaServiceModules
{
    private readonly HttpClient _client;
    public MediaServiceModules(IHttpClientFactory factory) => _client = factory.CreateClient("MediaServiceClient");

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

    public async Task<Response> UploadsAsync(IEnumerable<IFormFile>? files, CancellationToken ct)
    {
        using var formData = new MultipartFormDataContent();

        if (files != null)
            files.ToList().ForEach(f => formData.Add(new StreamContent(f.OpenReadStream()), "Files", f.FileName));
        else
        {
            var nullContent = new ByteArrayContent(Array.Empty<byte>());
            nullContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
            {
                Name = "Files",
                FileName = "null",
                Size = 0
            };
            formData.Add(nullContent);
        }

        var (method, path) = MediaServiceEndpoints.Uploads;
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
            JsonConvert.SerializeObject(new { Base64Content = base64Content ?? string.Empty }),
            Encoding.UTF8,
            "application/json"
        );

        using var responseMessage = await _client.SendAsync(requestMessage, ct);
        var responseBodyString = await responseMessage.Content.ReadAsStringAsync(ct);
        return JsonConvert.DeserializeObject<Response>(responseBodyString)!;
    }

    public async Task<(Stream stream, string contentType)> PreviewAsync(string id, CancellationToken ct)
    {
        var (method, path) = MediaServiceEndpoints.Preview(id);
        var requestMessage = new HttpRequestMessage(method, path);
        var responseMessage = await _client.SendAsync(requestMessage, ct);

        var contentType = responseMessage.Content.Headers.ContentType!.ToString();
        var stream = await responseMessage.Content.ReadAsStreamAsync(ct);

        return (stream, contentType);
    }

    public async Task<(Stream stream, string contentType)> PreviewThumbnailAsync(string id, CancellationToken ct)
    {
        var (method, path) = MediaServiceEndpoints.PreviewThumbnail(id);
        var requestMessage = new HttpRequestMessage(method, path);
        var responseMessage = await _client.SendAsync(requestMessage, ct);

        var contentType = responseMessage.Content.Headers.ContentType!.ToString();
        var stream = await responseMessage.Content.ReadAsStreamAsync(ct);

        return (stream, contentType);
    }

    public async Task<(Stream stream, string fileName)> DownloadAsync(string id, CancellationToken ct)
    {
        var (method, path) = MediaServiceEndpoints.Download(id);
        var requestMessage = new HttpRequestMessage(method, path);
        var responseMessage = await _client.SendAsync(requestMessage, ct);
        
        var contentType = responseMessage.Content.Headers.ContentType?.MediaType;
        
        if (!contentType.IsSupportedMediaType()) throw new HttpException(415, "Unsupported file type.");

        var contentDisposition = responseMessage.Content.Headers.ContentDisposition;
        var stream = await responseMessage.Content.ReadAsStreamAsync(ct);
        string fileName;

        if (contentDisposition != null && !string.IsNullOrEmpty(contentDisposition.FileName))
            fileName = contentDisposition.FileName;
        else if (contentDisposition != null && !string.IsNullOrEmpty(contentDisposition.FileNameStar))
        {
            var match = Regex.Match(contentDisposition.FileNameStar, @"^[^']*'[^']*'(.+)$");
            if (match.Success)
            {
                var encodedFilename = match.Groups[1].Value;
                var decodedFilename = Uri.UnescapeDataString(encodedFilename);
                fileName = decodedFilename;
            }
            else fileName = contentDisposition.FileNameStar;
        }
        else throw new HttpException(415, "Unsupported file type.");


        return (stream, fileName);
    }
    
    public async Task<(Stream stream, string fileName)> DownloadThumbAsync(string id, CancellationToken ct)
    {
        var (method, path) = MediaServiceEndpoints.DownloadThumbnail(id);
        var requestMessage = new HttpRequestMessage(method, path);
        var responseMessage = await _client.SendAsync(requestMessage, ct);
        
        var contentType = responseMessage.Content.Headers.ContentType?.MediaType;
        
        if (!contentType.IsSupportedMediaType()) throw new HttpException(415, "Unsupported file type.");

        var contentDisposition = responseMessage.Content.Headers.ContentDisposition;
        var stream = await responseMessage.Content.ReadAsStreamAsync(ct);
        string fileName;

        if (contentDisposition != null && !string.IsNullOrEmpty(contentDisposition.FileName))
            fileName = contentDisposition.FileName;
        else if (contentDisposition != null && !string.IsNullOrEmpty(contentDisposition.FileNameStar))
        {
            var match = Regex.Match(contentDisposition.FileNameStar, @"^[^']*'[^']*'(.+)$");
            if (match.Success)
            {
                var encodedFilename = match.Groups[1].Value;
                var decodedFilename = Uri.UnescapeDataString(encodedFilename);
                fileName = decodedFilename;
            }
            else fileName = contentDisposition.FileNameStar;
        }
        else throw new HttpException(415, "Unsupported file type.");


        return (stream, fileName);
    }
}