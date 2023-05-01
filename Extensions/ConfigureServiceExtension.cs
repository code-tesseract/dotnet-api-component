// ReSharper disable MemberCanBePrivate.Global

using Component.Base;
using Component.Externals.MediaService;
using Component.Settings;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Component.Extensions;

public static class ConfigureServiceExtension
{
	private static IConfiguration _configuration = null!;

	public static void AddGeneralServices(this WebApplicationBuilder builder, IConfiguration configuration)
	{
		var service = builder.Services;
		_configuration = configuration;

		service.AddController();
		service.AddRouting();
		service.AddAccessor();
		service.AddSettings();
		service.AddMediator();
		service.AddValidationBehavior();
		service.AddConfigureApiBehaviorOptions();
		service.AddMediaServiceClient();
	}

	public static void AddController(this IServiceCollection service)
		=> service.AddControllers()
				  .AddNewtonsoftJson(options =>
				  {
					  options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
					  options.SerializerSettings.ContractResolver = new DefaultContractResolver
						  { NamingStrategy = new DefaultNamingStrategy() };
				  });

	public static void AddRouting(this IServiceCollection service) =>
		service.AddRouting(opt => opt.LowercaseUrls = true);

	public static void AddAccessor(this IServiceCollection service) => service.AddHttpContextAccessor();

	public static void AddSettings(this IServiceCollection service)
		=> service.Configure<AppSetting>(_configuration.GetSection("AppSetting"))
				  .Configure<MediaServiceSetting>(_configuration.GetSection("MediaServiceSetting"));

	public static void AddMediator(this IServiceCollection service)
		=> service.AddMediatR(AppDomain.CurrentDomain.GetAssemblies());

	public static void AddValidationBehavior(this IServiceCollection service) =>
		service.AddTransient(typeof(IPipelineBehavior<,>), typeof(BaseValidationBehavior<,>));

	public static void AddConfigureApiBehaviorOptions(this IServiceCollection service) =>
		service.Configure<ApiBehaviorOptions>(options => options.SuppressModelStateInvalidFilter = true);

	public static void AddMediaServiceClient(this IServiceCollection sc)
	{
		var sp           = sc.BuildServiceProvider();
		var mediaSetting = sp.GetRequiredService<IOptions<MediaServiceSetting>>().Value;

		sc.AddScoped<MediaServiceHandler>();
		sc.AddScoped<IMediaServiceModules, MediaServiceModules>();
		sc.AddHttpClient("MediaServiceClient", m => m.BaseAddress = new Uri(mediaSetting.Url))
		  .AddHttpMessageHandler<MediaServiceHandler>();
	}
}