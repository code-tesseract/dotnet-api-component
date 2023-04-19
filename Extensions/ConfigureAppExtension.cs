using Component.Base;
using Component.Helpers;
using Component.Middlewares;
using Component.Models;
using Component.Settings;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Component.Extensions;

public static class ConfigureAppExtension
{
    public static void AddGeneralPipelines(this IApplicationBuilder app)
    {
        app.ApplicationServices.ConfigureDirectoryHelper();
        app.ApplicationServices.ConfigureUriHelper();

        app.UseMiddleware<ExceptionMiddleware>();
        app.UseRouting();
        app.UseEndpoints(endpoints => endpoints.MapControllers());
        app.UseForwardedHeaders(new ForwardedHeadersOptions
            { ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto });
        app.UseCors(builder => builder
            .SetIsOriginAllowed(_ => true)
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials()
        );
        
        ValidatorOptions.Global.LanguageManager = new BaseLanguageManager();
    }
    
    public static void UseDefaultEndpoint(this WebApplication app)
    {
        var appSetting = app.Services.GetRequiredService<IOptions<AppSetting>>().Value;
        app.MapGet("~/", (HttpContext context)
            => JsonConvert.SerializeObject(
                new Response(
                    name: appSetting.AppName,
                    message: $"{appSetting.AppName} is running",
                    data: $"You are accessing this endpoint from {IdentityHelper.GetClientIp(context)}"
                )
            )
        );
    }
}