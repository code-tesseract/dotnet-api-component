using MediatR;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Component.Externals.MediaService.Commands;

public class DownloadCommand: IRequest<(Stream stream, string fileName)>
{
    public DownloadCommand(string id) => Id = id;
    public string Id { get; set; }

    public class DownloadHandler : IRequestHandler<DownloadCommand, (Stream stream, string fileName)>
    {
        private readonly IMediaServiceModules _ms;
        public DownloadHandler(IMediaServiceModules ms) => _ms = ms;

        public async Task<(Stream stream, string fileName)> Handle(DownloadCommand request, CancellationToken ct)
            => await _ms.DownloadAsync(request.Id, ct);
    }
    
}