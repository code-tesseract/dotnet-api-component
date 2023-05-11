using System.Globalization;
using Component.Base;
using Component.Helpers;
using Component.Middlewares;
using Component.Models;
using Component.Settings;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Component.Extensions;

public static class ConfigureAppExtension
{
	public static void AddGeneralPipelines(this IApplicationBuilder app)
	{
		app.UseCors(builder => builder
			.SetIsOriginAllowed(_ => true)
			.AllowAnyMethod()
			.AllowAnyHeader()
			.AllowCredentials()
		);
		
		app.ApplicationServices.ConfigureDirectoryHelper();
		app.ApplicationServices.ConfigureUriHelper();

		app.AddLocalization();
		app.UseMiddleware<ExceptionMiddleware>();
		app.UseRouting();
		app.UseEndpoints(endpoints => endpoints.MapControllers());
		app.UseForwardedHeaders(new ForwardedHeadersOptions
		{
			ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
		});

		ValidatorOptions.Global.LanguageManager = new BaseLanguageManager();
	}

	private static void AddLocalization(this IApplicationBuilder app)
	{
		var supportedCultures = new[] { new CultureInfo("en-US"), new CultureInfo("id-ID") };
		app.UseRequestLocalization(new RequestLocalizationOptions
		{
			DefaultRequestCulture = new RequestCulture(supportedCultures[0]),
			SupportedCultures     = supportedCultures,
			SupportedUICultures   = supportedCultures
		});
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