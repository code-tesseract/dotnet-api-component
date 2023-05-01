using FluentValidation;
using MediatR;

namespace Component.Base;

public class BaseValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
	where TRequest : IRequest<TResponse>
{
	private readonly IEnumerable<IValidator<TRequest>> _validators;

	public BaseValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
	{
		_validators = validators;
	}

	public async Task<TResponse> Handle(TRequest          request, RequestHandlerDelegate<TResponse> next,
										CancellationToken cancellationToken)
	{
		if (!_validators.Any()) return await next();

		var context = new ValidationContext<TRequest>(request);

		var validationTasks   = _validators.Select(x => x.ValidateAsync(context, cancellationToken));
		var validationResults = await Task.WhenAll(validationTasks);
		var errorsDictionary = validationResults
							   .SelectMany(x => x.Errors)
							   .Where(x => x != null)
							   .GroupBy(
								   x => x.PropertyName,
								   x => x.ErrorMessage,
								   (propertyName, errorMessages) => new
								   {
									   Key    = propertyName,
									   Values = errorMessages.Distinct().ToArray()
								   })
							   .ToDictionary(x => x.Key, x => x.Values);

		if (errorsDictionary.Any())
			throw new Exceptions.ValidationException(errorsDictionary);

		return await next();
	}
}