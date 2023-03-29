using Component.Base;
using Component.Exceptions;
using Component.Externals.MediaService.Commands;
using Microsoft.AspNetCore.Mvc;

namespace Component.Externals.MediaService;

[Route("media")]
public class MediaServiceController : BaseController
{
    [HttpGet("requirements")]
    public async Task<IActionResult> Requirements(CancellationToken ct) =>
        Ok(await Mediator.Send(new GetRequirementsCommand(), ct));

    [HttpPost("upload")]
    public async Task<IActionResult> Upload([FromForm] UploadCommand command, CancellationToken ct)
        => Ok(await Mediator.Send(command, ct));

    [HttpPost("upload-base64")]
    public async Task<IActionResult> UploadBase64([FromBody] UploadBase64Command? command, CancellationToken ct)
    {
        if (command == null) throw new HttpException(400, "Please provide the body of your request.");
        return Ok(await Mediator.Send(command, ct));
    } 

    // [HttpPost("upload-base64")]
    // public IActionResult UploadBase64(JsonElement json, CancellationToken ct)
    // {
    //     // return Ok(await _media.UploadBase64Async(base64Content, ct));
    //     var base64Content = json.GetProperty("base64Content").GetString();
    //     throw new HttpException(400, $"{base64Content}");
    //     // return Ok(base64Content);
    // }
    //
    // public class UploadBase64Request
    // {
    //     public string Base64Content { get; set; } = null!;
    // }
}