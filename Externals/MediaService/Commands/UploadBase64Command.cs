using Component.Models;
using MediatR;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Component.Externals.MediaService.Commands;

public class UploadBase64Command : IRequest<Response>
{
	public string? Base64Content { get; set; }

	public class UploadBase64Handler : IRequestHandler<UploadBase64Command, Response>
	{
		private readonly IMediaServiceModules _ms;
		public UploadBase64Handler(IMediaServiceModules ms) => _ms = ms;

		public async Task<Response> Handle(UploadBase64Command request, CancellationToken ct)
			=> await _ms.UploadBase64Async(request.Base64Content, ct);
	}
}