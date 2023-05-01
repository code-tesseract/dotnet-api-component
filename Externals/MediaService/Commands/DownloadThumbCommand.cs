using MediatR;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Component.Externals.MediaService.Commands;

public class DownloadThumbCommand : IRequest<(Stream stream, string fileName)>
{
	public DownloadThumbCommand(string id) => Id = id;
	public string Id { get; set; }

	public class DownloadThumbHandler : IRequestHandler<DownloadThumbCommand, (Stream stream, string fileName)>
	{
		private readonly IMediaServiceModules _ms;
		public DownloadThumbHandler(IMediaServiceModules ms) => _ms = ms;

		public async Task<(Stream stream, string fileName)> Handle(DownloadThumbCommand request, CancellationToken ct)
			=> await _ms.DownloadThumbAsync(request.Id, ct);
	}
}