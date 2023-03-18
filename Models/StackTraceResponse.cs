// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Component.Models;

public class StackTraceResponse
{
    public string? Message { get; set; }
    public string? Type { get; set; }
    public string? Method { get; set; }
    public string? File { get; set; }
    public int? Line { get; set; }
    public List<string>? Trace { get; set; }

    public StackTraceResponse(string? message, string? type, string? method, string? file, int? line,
        List<string>? trace)
    {
        Message = message;
        Type = type;
        Method = method;
        File = file;
        Line = line;
        Trace = trace;
    }
}