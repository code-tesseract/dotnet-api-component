using Component.Base;
using Component.Entities;
using Component.Exceptions;
using Component.Helpers;
using Component.Settings;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Component.Attributes;

#pragma warning restore CA1018
public class ClientHeadersAttribute : Attribute, IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var allowAnonymous = context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any();
        if (!allowAnonymous)
        {
            var db = context.HttpContext.RequestServices.GetService<BaseDbContext>();
            if (db == null) throw new HttpException(503, "System is unavailable right now, please contact help desk.");

            var appSetting = context.HttpContext.RequestServices
                .GetRequiredService<IOptions<AppSetting>>().Value;
            var appClientSetting = appSetting.AppClientSetting;

            if (appSetting == null || appClientSetting == null)
                throw new HttpException(503, "Client is not set in this app. Please contact help desk.");

            if (!context.HttpContext.Request.Headers.TryGetValue("X-Client-Key", out var clientKey))
                throw new HttpException(400, "Please provide the security of client key");

            if (!context.HttpContext.Request.Headers.TryGetValue("X-Client-Secret", out var clientSecret))
                throw new HttpException(400, "Please provide the security of client secret");

            var client = await db.Client
                .Include(c => c.ClientWhitelists)
                .FirstOrDefaultAsync(c => c.Key == clientKey.ToString() && c.Secret == clientSecret.ToString());

            if (client == null)
                throw new HttpException(400, "Either one or both of your client key and client secret is invalid.");

            if (client.Status != Client.StatusActive)
                throw new HttpException(400, $"Your app status is {client.Status}. Please contact help desk.");

            /* check client whitelist when not null */
            if (client.ClientWhitelists?.Count > 0)
            {
                var ip = IdentityHelper.GetClientIp(context.HttpContext);
                var isAllowedIp = client.ClientWhitelists
                    .Any(cw => cw.Ip == ip && cw.Status.Equals(ClientWhitelist.StatusActive));
                if (!isAllowedIp) throw new HttpException(400, "Your app can only be accessed from specific IP.");
            }
        }

        await next();
    }
}