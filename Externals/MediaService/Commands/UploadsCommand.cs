using Component.Models;
using MediatR;
using Microsoft.AspNetCore.Http;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Component.Externals.MediaService.Commands;

public class UploadsCommand : IRequest<Response>
{
    public IEnumerable<IFormFile>? Files { get; set; }
    
    public class UploadsHandler : IRequestHandler<UploadsCommand, Response>
    {
        private readonly IMediaService _ms;
        public UploadsHandler(IMediaService ms) => _ms = ms;

        public async Task<Response> Handle(UploadsCommand request, CancellationToken ct)
            => await _ms.UploadsAsync(request.Files, ct);
    }
}