using System.Text.RegularExpressions;
using FluentValidation;

namespace Component.Validators;

public class PasswordValidator : AbstractValidator<string>
{
    private string PropertyName { get; set; }

    public PasswordValidator(string propertyName)
    {
        PropertyName = propertyName;
        RuleFor(password => password)
            .Length(8, 100)
            .WithMessage($"{PropertyName} must be between 8 and 100 characters/numbers long.")
            .Must(password => Regex.IsMatch(password, @"[A-Z]"))
            .WithMessage($"{PropertyName} must contain at least one uppercase letter.")
            .Must(password => Regex.IsMatch(password, @"\d"))
            .WithMessage($"{PropertyName} must contain at least one digit.");
    }
}