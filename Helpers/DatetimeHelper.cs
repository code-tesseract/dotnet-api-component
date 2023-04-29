namespace Component.Helpers;

public static class DatetimeHelper
{
    private static int ToUnixTimeSeconds(DateTime date)
    {
        var point = new DateTime(1970, 1, 1);
        var time = date.Subtract(point);

        return (int)time.TotalSeconds;
    }

    public static int ToUnixTimeSeconds(DateTime? dateTime = null) => ToUnixTimeSeconds(dateTime ?? DateTime.UtcNow);

    public static DateTime ToDatetimeFromUnixTimeSeconds(int unixTimeSeconds) =>
        new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)
            .AddSeconds(unixTimeSeconds)
            .ToLocalTime();

    public static DateTime UtcLocalNow() => ToDatetimeFromUnixTimeSeconds(ToUnixTimeSeconds());
}