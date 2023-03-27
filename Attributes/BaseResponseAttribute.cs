using Component.Helpers;
using Component.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

// ReSharper disable MemberCanBePrivate.Global

namespace Component.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class BaseResponseAttribute : Attribute, IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var sw = StopwatchHelper.StartNew();
        var resultContext = await next();

        if (resultContext.Exception is null)
        {
            var response = new Response();

            var responseObjectResult = resultContext.Result as ObjectResult;
            var responseBodyObject = responseObjectResult?.Value;

            if (responseBodyObject != null)
            {
                var responseBodyString = !responseBodyObject.ToString().IsValidJson()
                    ? JsonConvert.SerializeObject(responseBodyObject, ResponseHelper.DefaultJsonSetting())
                    : responseBodyObject.ToString();

                if (responseBodyString != null)
                {
                    var responseBodyContent = JsonConvert.DeserializeObject<dynamic>(responseBodyString);
                    if (responseBodyContent != null)
                    {
                        Type type = responseBodyContent.GetType();
                        if (type == typeof(JObject))
                            response = JsonConvert.DeserializeObject<Response>(responseBodyString) ??
                                       new Response(responseBodyContent);
                        else response = new Response(responseBodyContent);
                    }
                }
            }

            response.RequestTime = sw.GetSecondElapsedTime();
            resultContext.HttpContext.Response.StatusCode = response.Status;
            resultContext.Result = new ObjectResult(response);
        }
        sw.Stop();
    }
}