using FluentValidation.Resources;

// ReSharper disable ArrangeObjectCreationWhenTypeNotEvident

namespace Component.Base;

/*
 * FluentValidation
 * 
 * NotEmptyValidator: Indicates that a property is required and cannot be empty or null.
 * NotNullValidator: Indicates that a property cannot be null.
 * LengthValidator: Indicates that a property must have a specific length or fall within a specific length range.
 * ExactLengthValidator: Indicates that a property must have an exact length.
 * MinimumLengthValidator: Indicates that a property must have a minimum length.
 * MaximumLengthValidator: Indicates that a property must have a maximum length.
 * InclusiveBetweenValidator: Indicates that a property must fall within an inclusive range of values.
 * ExclusiveBetweenValidator: Indicates that a property must fall within an exclusive range of values.
 * RegularExpressionValidator: Indicates that a property must match a specific regular expression pattern.
 * EmailValidator: Indicates that a property must be a valid email address.
 * CreditCardValidator: Indicates that a property must be a valid credit card number.
 * GreaterThanValidator: Indicates that a property must be greater than a specified value.
 * GreaterThanOrEqualValidator: Indicates that a property must be greater than or equal to a specified value.
 * LessThanValidator: Indicates that a property must be less than a specified value.
 * LessThanOrEqualValidator: Indicates that a property must be less than or equal to a specified value.
 * EqualValidator: Indicates that a property must be equal to a specified value.
 * NotEqualValidator: Indicates that a property must not be equal to a specified value.
 * Must: Indicates that a custom validation function must be satisfied.
 * MustAsync: Indicates that a custom asynchronous validation function must be satisfied.
*/

public class BaseLanguageManager : LanguageManager
{
	private const string DefaultLanguage = "en-US";

	private class TranslationModel
	{
		public TranslationModel(string key, string value, string language = DefaultLanguage)
		{
			Key      = key;
			Value    = value;
			Language = language;
		}

		public string Language { get; set; }
		public string Key      { get; set; }
		public string Value    { get; set; }
	}

	private static readonly List<TranslationModel> ValidationMessages = new()
	{
		new("NotEmptyValidator", "{PropertyName} is required. Please provide a valid input."),
		new("NotNullValidator", "{PropertyName} cannot be null. Please provide a valid value."),
		new("LengthValidator", "{PropertyName} must be between {MinLength} and {MaxLength} characters/numbers long."),
		new("MinimumLengthValidator", "{PropertyName} must be at least {MinLength} characters/numbers long."),
		new("MaximumLengthValidator", "{PropertyName} must not exceed {MaxLength} characters/numbers long."),
		new("GreaterThanValidator", "{PropertyName} must be greater than {ComparisonValue}."),
		new("GreaterThanOrEqualValidator", "{PropertyName} must be greater than or equal to {ComparisonValue}."),
		new("LessThanValidator", "{PropertyName} must be less than {ComparisonValue}."),
		new("LessThanOrEqualValidator", "{PropertyName} must be less than or equal to {ComparisonValue}."),
		new("EqualValidator", "{PropertyName} must be equal to {ComparisonProperty}."),
		new("NotEqualValidator", "{PropertyName} must not be equal to {ComparisonProperty}."),
		new("ExclusiveBetweenValidator", "{PropertyName} must be between {ComparisonValue}, exclusive."),
		new("InclusiveBetweenValidator", "{PropertyName} must be between {ComparisonValue}, inclusive."),
		new("EmailValidator", "Please provide a valid email address.")
	};

	public BaseLanguageManager() => ValidationMessages.ForEach(fv => AddTranslation(fv.Language, fv.Key, fv.Value));
}