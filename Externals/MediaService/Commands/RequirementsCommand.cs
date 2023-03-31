using Component.Models;
using MediatR;

namespace Component.Externals.MediaService.Commands;

public class RequirementsCommand : IRequest<Response>
{
    public class GetRequirementHandler : IRequestHandler<RequirementsCommand, Response>
    {
        private readonly IMediaServiceModules _ms;
        public GetRequirementHandler(IMediaServiceModules ms) => _ms = ms;

        public async Task<Response> Handle(RequirementsCommand request, CancellationToken ct)
            => await _ms.Requirements(ct);
    }
}