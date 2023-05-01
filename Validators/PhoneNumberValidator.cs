using System.Text.RegularExpressions;
using FluentValidation;

namespace Component.Validators;

public class PhoneNumberValidator : AbstractValidator<string>
{
	private const string RegexPattern = @"^(?:\+62|62|0)8(?:11|12|13|14|15|16|17|18|19|21|22|23|28|31|38|51|52|
53|55|56|57|58|59|77|78|79|81|82|83|84|87|88|89|95|96|97|98|99|681)[0-9]{6,9}$";

	public string PropertyName { get; set; }

	public PhoneNumberValidator(string propertyName)
	{
		PropertyName = propertyName;
		RuleFor(phoneNumber => phoneNumber)
			.Length(5, 20)
			.WithMessage($"{PropertyName} must be between 5 and 100 characters/numbers long.")
			.Must(phoneNumber => Regex.IsMatch(phoneNumber, RegexPattern))
			.WithMessage($"{PropertyName} not listed in any registered provider.");
	}
}