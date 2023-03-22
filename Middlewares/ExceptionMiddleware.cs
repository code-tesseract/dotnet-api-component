using System.Diagnostics;
using Component.Enums;
using Component.Exceptions;
using Component.Helpers;
using Component.Models;
using Component.Settings;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Component.Middlewares;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly AppSetting _appSetting;

    public ExceptionMiddleware(RequestDelegate next, IOptions<AppSetting> appSetting)
    {
        _next = next;
        _appSetting = appSetting.Value;
    }

    public async Task Invoke(HttpContext context)
    {
        var sw = StopwatchHelper.StartNew();
        try
        {
            if (context.Request.ContentType != "application/json" && context.Request.Method == HttpMethods.Post)
            {
                context.Response.StatusCode = StatusCodes.Status415UnsupportedMediaType;
                context.Response.ContentType = "application/json";

                var response = new Response(
                    message: ResponseMessageEnum.UnsupportedMediaType.GetDescription(),
                    code: (int)ResponseCodeEnum.Failed,
                    status: StatusCodes.Status415UnsupportedMediaType,
                    requestTime: sw.GetSecondElapsedTime(),
                    data: null
                );

                await context.Response.WriteAsync(JsonConvert.SerializeObject(response));
            }
            else
            {
                await _next.Invoke(context);
                if (!context.Response.HasStarted)
                {
                    Response? response;
                    switch (context.Response.StatusCode)
                    {
                        case StatusCodes.Status404NotFound:
                            response = new Response(
                                message: ResponseMessageEnum.NotFound.GetDescription(),
                                code: (int)ResponseCodeEnum.Failed,
                                status: StatusCodes.Status404NotFound,
                                requestTime: sw.GetSecondElapsedTime(),
                                data: null
                            );
                            await context.Response.WriteAsync(JsonConvert.SerializeObject(response));
                            return;
                        case StatusCodes.Status405MethodNotAllowed:
                            response = new Response(
                                message: ResponseMessageEnum.MethodNotAllowed.GetDescription(),
                                code: (int)ResponseCodeEnum.Failed,
                                status: StatusCodes.Status405MethodNotAllowed,
                                requestTime: sw.GetSecondElapsedTime(),
                                data: null
                            );
                            await context.Response.WriteAsync(JsonConvert.SerializeObject(response));
                            return;
                    }
                }
            }
        }
        catch (Exception exception)
        {
            Console.WriteLine($"Error: {exception}");

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

                    var stackTraceResponse =
                        new StackTraceResponse(exceptionMessage, type, method, file, line, traceList);
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
                requestTime: sw.GetSecondElapsedTime(),
                data: null,
                errors: new { Validation = validation, StackTrace = trace }
            );

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = status;
            await context.Response.WriteAsync(JsonConvert.SerializeObject(response));
        }
        finally
        {
            sw.Stop();
        }
    }

    private static IReadOnlyDictionary<string, string[]> GetValidationErrors(Exception exception)
    {
        IReadOnlyDictionary<string, string[]> errors = null!;
        if (exception is ValidationException validationException) errors = validationException.ErrorsDictionary;
        return errors;
    }
}