using System;

public class TimeHelper
{
    public static DateTime UnixTimeToUtc(double unixTime)
    {
        var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var timeSpan = TimeSpan.FromSeconds(unixTime);

        return epoch.Add(timeSpan).ToUniversalTime();
    }
}