using System.Globalization;

namespace Component.Helpers;

public static class StringHelper
{
    public static string ToTitleCase(this string str) => CultureInfo.CurrentCulture.TextInfo.ToTitleCase(str);
}