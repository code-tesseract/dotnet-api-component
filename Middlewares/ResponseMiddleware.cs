using System.Diagnostics;
using Component.Enums;
using Component.Exceptions;
using Component.Helpers;
using Component.Models;
using Component.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Component.Middlewares;

public class ResponseMiddleware : IMiddleware
{
    private readonly ILogger<ResponseMiddleware> _log;
    private static AppSetting _appSetting = null!;

    public ResponseMiddleware(ILogger<ResponseMiddleware> log, IOptions<AppSetting> appSetting)
    {
        _log = log;
        _appSetting = appSetting.Value;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        // var sw = StopwatchHelper.StartNew();
        var originalBodyStream = context.Response.Body;
        using var memStream = new MemoryStream();
        try
        {
            context.Response.Body = memStream;
            await next.Invoke(context);
            context.Response.Body = originalBodyStream;

            // var requestTime = sw.GetSecondElapsedTime();
            var requestTime = 0;
            var status = context.Response.StatusCode;

            /* Handle response when success */
            var successStatuses = new[]
            {
                StatusCodes.Status200OK,
                StatusCodes.Status201Created,
                StatusCodes.Status202Accepted
            };

            if (successStatuses.Contains(status))
            {
                var responseBodyString = await ResponseHelper.FormatResponse(memStream);
                await HandleSuccessRequestAsync(context, responseBodyString, status, requestTime);
            }
            else await HandleNotSuccessRequestAsync(context, status, requestTime);
        }
        catch (Exception ex)
        {
            _log.LogError("{Errors}", ex);
            var requestTime = 0;
            await HandleExceptionAsync(context, ex, requestTime);
            memStream.Seek(0, SeekOrigin.Begin);
            await memStream.CopyToAsync(originalBodyStream);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception, double? requestTime)
    {
        string message;
        int status;
        IReadOnlyDictionary<string, string[]> validation = new Dictionary<string, string[]>();
        var trace = new object();

        switch (exception)
        {
            case ValidationException ve:
                message = ve.Message;
                status = StatusCodes.Status400BadRequest;
                validation = GetValidationErrors(exception);
                break;
            case HttpRequestException hre:
                message = hre.Message;
                status = hre.StatusCode.GetHashCode();
                break;
            case HttpException he:
                message = he.Message;
                status = he.Status;
                break;
            default:
                var exceptionMessage = exception.GetBaseException().Message;
                var type = exception.GetBaseException().GetType().FullName;
                var stackTrace = new StackTrace(exception, true);
                var frame = stackTrace.GetFrame(0);
                var method = frame?.GetMethod()?.Name;
                var line = frame?.GetFileLineNumber();
                var file = frame?.GetFileName();

                var traceList = ResponseHelper.GetExceptionTraceList(exception);
                if (traceList.Count > 0)
                {
                    var firstTrace = traceList[0].Split(Environment.NewLine).FirstOrDefault();
                    if (firstTrace != null)
                    {
                        file = ResponseHelper.GetFirstErrorPathFile(firstTrace);
                        line = ResponseHelper.GetFirstErrorLineNumber(firstTrace);
                    }
                }

                var stackTraceResponse = new StackTraceResponse(exceptionMessage, type, method, file, line, traceList);
                message = ResponseMessageEnum.Unhandled.GetDescription();
                status = StatusCodes.Status500InternalServerError;

                /* Show stack trace when unhandled request & response when app env is `Development` */
                if (_appSetting.AppEnv != null && _appSetting.AppEnv == Environments.Development)
                    trace = stackTraceResponse;
                break;
        }

        var response = new Response(
            message: message,
            code: (int)ResponseCodeEnum.Failed,
            status: status,
            requestTime: requestTime,
            data: null,
            errors: new { Validation = validation, StackTrace = trace }
        );

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = status;
        return context.Response.WriteAsync(JsonConvert.SerializeObject(response));
    }

    private static Task HandleNotSuccessRequestAsync(HttpContext context, int status, double? requestTime)
    {
        var message = status switch
        {
            StatusCodes.Status404NotFound => ResponseMessageEnum.NotFound.GetDescription(),
            StatusCodes.Status204NoContent => ResponseMessageEnum.NotContent.GetDescription(),
            StatusCodes.Status405MethodNotAllowed => ResponseMessageEnum.MethodNotAllowed.GetDescription(),
            _ => "Your request cannot be processed. Please contact a support."
        };

        var response = JsonConvert.SerializeObject(new Response(
            message: message,
            code: (int)ResponseCodeEnum.Failed,
            status: status,
            requestTime: requestTime,
            data: null
        ));

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = status;
        
        return context.Response.WriteAsync(response);
    }

    private static Task HandleSuccessRequestAsync(HttpContext context, object body, int status,
        double? requestTime)
    {
        Response? response;

        var bodyText = !body.ToString().IsValidJson() ? JsonConvert.SerializeObject(body) : body.ToString();
        var bodyContent = JsonConvert.DeserializeObject<dynamic>(bodyText!);

        Type type = bodyContent?.GetType()!;

        if (type == typeof(JObject))
        {
            response = JsonConvert.DeserializeObject<Response>(bodyText!);
            if (response != null) response.RequestTime = requestTime;
            else
                response = new Response(
                    message: ResponseMessageEnum.Success.GetDescription(),
                    code: (int)ResponseCodeEnum.Success,
                    status: status,
                    requestTime: requestTime,
                    data: bodyContent
                );
        }
        else
            response = new Response(
                message: ResponseMessageEnum.Success.GetDescription(),
                code: (int)ResponseCodeEnum.Success,
                status: status,
                requestTime: requestTime,
                data: bodyContent
            );

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = response.Status;

        return context.Response.WriteAsync(JsonConvert.SerializeObject(response));
    }

    private static IReadOnlyDictionary<string, string[]> GetValidationErrors(Exception exception)
    {
        IReadOnlyDictionary<string, string[]> errors = null!;
        if (exception is ValidationException validationException) errors = validationException.ErrorsDictionary;
        return errors;
    }
}