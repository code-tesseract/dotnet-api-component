using MediatR;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Component.Externals.MediaService.Commands;

public class PreviewThumbCommand : IRequest<(Stream stream, string contentType)>
{
	public PreviewThumbCommand(string id) => Id = id;
	public string Id { get; set; }

	public class PreviewThumbHandler : IRequestHandler<PreviewThumbCommand, (Stream stream, string contentType)>
	{
		private readonly IMediaServiceModules _ms;
		public PreviewThumbHandler(IMediaServiceModules ms) => _ms = ms;

		public async Task<(Stream stream, string contentType)> Handle(PreviewThumbCommand request, CancellationToken ct)
			=> await _ms.PreviewThumbnailAsync(request.Id, ct);
	}
}