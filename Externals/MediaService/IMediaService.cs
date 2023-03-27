using Component.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Component.Externals.MediaService;

public interface IMediaService
{
    Task<Response> Requirements(CancellationToken ct);
    Task<Response> UploadAsync(IFormFile file, CancellationToken ct);
    Task<Response> UploadBase64Async(string base64Content, CancellationToken ct);
    Task<FileStreamResult> PreviewAsync(Guid mediaId, CancellationToken ct);
    Task<FileStreamResult> PreviewThumbnailAsync(Guid mediaId, CancellationToken ct);
}