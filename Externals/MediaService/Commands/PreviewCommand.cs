using MediatR;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Component.Externals.MediaService.Commands;

public class PreviewCommand : IRequest<(Stream stream, string contentType)>
{
    public PreviewCommand(string id) => Id = id;
    public string Id { get; set; }

    public class PreviewHandler : IRequestHandler<PreviewCommand, (Stream stream, string contentType)>
    {
        private readonly IMediaService _ms;
        public PreviewHandler(IMediaService ms) => _ms = ms;

        public async Task<(Stream stream, string contentType)> Handle(PreviewCommand request, CancellationToken ct)
            => await _ms.PreviewAsync(request.Id, ct);
    }
}