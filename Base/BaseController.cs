using Component.Attributes;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable NullCoalescingConditionIsAlwaysNotNullAccordingToAPIContract
#pragma warning disable CS8618
#pragma warning disable CS8603
#pragma warning disable CS8601

namespace Component.Base;

[ApiController]
[ClientHeaders]
[BaseResponse]
public class BaseController : ControllerBase
{
    private IMediator _mediator;
    protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetService<IMediator>();
}