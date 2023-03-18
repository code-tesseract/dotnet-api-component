// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable ClassNeverInstantiated.Global

namespace Component.Exceptions;

public class HttpException : Exception
{
    public int Status { get; set; }

    public HttpException(int status, string message) : base(message)
    {
        Status = status;
    }

    public HttpException(Exception? ex, int status = 500) : base(ex?.Message)
    {
        Status = status;
    }
}