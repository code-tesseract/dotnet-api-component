using FluentValidation;
using FluentValidation.Validators;

namespace Component.Base;

public static class BaseRuleBuilders
{
    public static IRuleBuilderOptions<T, TProperty> Required<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder)
        => ruleBuilder.SetValidator(new NotEmptyValidator<T, TProperty>());
}