using Component.Attributes;
using Component.Base;
using Component.Exceptions;
using Component.Externals.MediaService.Commands;
using Microsoft.AspNetCore.Mvc;

namespace Component.Externals.MediaService;

[Route("external/media")]
public class MediaServiceController : BaseController
{
    [HttpGet("requirements")]
    public async Task<IActionResult> Requirements(CancellationToken ct) =>
        Ok(await Mediator.Send(new RequirementsCommand(), ct));

    [HttpPost("upload")]
    public async Task<IActionResult> Upload([FromForm] UploadCommand command, CancellationToken ct)
        => Ok(await Mediator.Send(command, ct));

    [HttpPost("uploads")]
    public async Task<IActionResult> Uploads([FromForm] UploadsCommand command, CancellationToken ct)
        => Ok(await Mediator.Send(command, ct));

    [HttpPost("upload-base64")]
    public async Task<IActionResult> UploadBase64([FromBody] UploadBase64Command? command, CancellationToken ct)
    {
        if (command == null) throw new HttpException(400, "Please provide the body of your request.");
        return Ok(await Mediator.Send(command, ct));
    }

    [AllowAnonymousBaseResponse]
    [HttpGet("preview/{id}")]
    public async Task<IActionResult> Preview([FromRoute] string id, CancellationToken ct)
    {
        var (stream, contentType) = await Mediator.Send(new PreviewCommand(id), ct);

        var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream, ct);
        memoryStream.Seek(0, SeekOrigin.Begin);

        return File(memoryStream, contentType);
    }
    
    [AllowAnonymousBaseResponse]
    [HttpGet("preview-thumbnail/{id}")]
    public async Task<IActionResult> PreviewThumbnail([FromRoute] string id, CancellationToken ct)
    {
        var (stream, contentType) = await Mediator.Send(new PreviewThumbCommand(id), ct);

        var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream, ct);
        memoryStream.Seek(0, SeekOrigin.Begin);

        return File(memoryStream, contentType);
    }
}