using System.Diagnostics;

namespace Component.Helpers;

public readonly struct StopwatchHelper
{
    private static readonly double STimestampToTicks = TimeSpan.TicksPerSecond / (double)Stopwatch.Frequency;
    private readonly long _startTimestamp;
    private readonly Stopwatch _stopwatch;
    
    private StopwatchHelper(long startTimestamp, Stopwatch stopwatch)
    {
        _startTimestamp = startTimestamp;
        _stopwatch = stopwatch;
    }
    
    public static StopwatchHelper StartNew()
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        return new StopwatchHelper(GetTimestamp(), stopwatch);
    }

    private static long GetTimestamp() => Stopwatch.GetTimestamp();

    private static TimeSpan GetElapsedTime(long startTimestamp, long endTimestamp)
    {
        var timestampDelta = endTimestamp - startTimestamp;
        var ticks = (long)(STimestampToTicks * timestampDelta);
        return new TimeSpan(ticks);
    }
    
    public string GetMillisecondElapsedTime() =>
        $"{Math.Round(GetElapsedTime(_startTimestamp, GetTimestamp()).TotalMilliseconds, 2)}".Replace(",", ".") + "ms";
    
    public double GetSecondElapsedTime() =>
        Math.Round(GetElapsedTime(_startTimestamp, GetTimestamp()).TotalSeconds, 2);

    public void Stop() => _stopwatch.Stop();
}
