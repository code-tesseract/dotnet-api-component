using System.ComponentModel;
using System.Globalization;

namespace Component.Helpers;

public static class StringEnumHelper
{
	public static string GetDescription<T>(this T e) where T : IConvertible
	{
		var description = string.Empty;

		if (e is not Enum) return description;
		var type   = e.GetType();
		var values = Enum.GetValues(type);

		foreach (int val in values)
		{
			if (val != e.ToInt32(CultureInfo.InvariantCulture)) continue;
			var memInfo               = type.GetMember(type.GetEnumName(val) ?? string.Empty);
			var descriptionAttributes = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
			if (descriptionAttributes.Length > 0)
			{
				description = ((DescriptionAttribute)descriptionAttributes[0]).Description;
			}

			break;
		}

		return description;
	}
}