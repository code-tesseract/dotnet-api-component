using Component.Models;
using MediatR;
using Microsoft.AspNetCore.Http;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Component.Externals.MediaService.Commands;

public class UploadCommand : IRequest<Response>
{
    public IFormFile? File { get; set; }

    public class UploadHandler : IRequestHandler<UploadCommand, Response>
    {
        private readonly IMediaServiceModules _ms;
        public UploadHandler(IMediaServiceModules ms) => _ms = ms;

        public async Task<Response> Handle(UploadCommand request, CancellationToken ct)
            => await _ms.UploadAsync(request.File, ct);
    }
}