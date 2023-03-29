using Component.Models;
using MediatR;

namespace Component.Externals.MediaService.Commands;

public class GetRequirementsCommand : IRequest<Response>
{
    public class GetRequirementHandler : IRequestHandler<GetRequirementsCommand, Response>
    {
        private readonly IMediaService _ms;
        public GetRequirementHandler(IMediaService ms) => _ms = ms;

        public async Task<Response> Handle(GetRequirementsCommand request, CancellationToken ct)
            => await _ms.Requirements(ct);
    }
}