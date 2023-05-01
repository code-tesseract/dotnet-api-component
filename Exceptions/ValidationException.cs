using Component.Enums;
using Component.Helpers;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global


namespace Component.Exceptions;

public sealed class ValidationException : Exception
{
	public ValidationException(IReadOnlyDictionary<string, string[]> errorsDictionary)
		: base(ResponseMessageEnum.ValidationError.GetDescription())
		=> ErrorsDictionary = errorsDictionary;

	public IReadOnlyDictionary<string, string[]> ErrorsDictionary { get; }
}