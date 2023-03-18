using Component.Helpers;
using Component.Middlewares;
using Component.Models;
using Component.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

// ReSharper disable MemberCanBePrivate.Global

namespace Component.Extensions;

public static class ComponentAppExtension
{
    public static void AddBaseComponentAppExtension(this IApplicationBuilder ab)
    {
        ab.AddComponentCors();
        ab.AddComponentForwardedHeaders();
        ab.AddComponentMiddlewares();
        ab.UseRouting();
        ab.UseEndpoints(endpoints => endpoints.MapControllers());
    }

    public static void AddComponentCors(this IApplicationBuilder ab) =>
        ab.UseCors(cpb => cpb
            .SetIsOriginAllowed(_ => true)
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());

    public static void AddComponentForwardedHeaders(this IApplicationBuilder ab) =>
        ab.UseForwardedHeaders(new ForwardedHeadersOptions
            { ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto });

    public static void AddComponentMiddlewares(this IApplicationBuilder ab) => ab.UseMiddleware<ExceptionMiddleware>();

    public static async Task AppRunAsync(this WebApplication wa)
    {
        var appSetting = wa.Services.GetRequiredService<IOptions<AppSetting>>().Value;
        var ub = new UriBuilder(appSetting.AppUrl);

        wa.MapGet("/", (HttpContext context) => JsonConvert.SerializeObject(new Response(
            name: appSetting.AppName,
            message: $"{appSetting.AppName} is running",
            data: $"You are accessing this endpoint from {IdentityHelper.GetClientIp(context)}"
        )));

        wa.Environment.EnvironmentName = appSetting.AppEnv;
        await wa.RunAsync(ub.Uri.AbsoluteUri);
    }
}