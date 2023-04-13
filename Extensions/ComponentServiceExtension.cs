using Component.Base;
using Component.Externals.MediaService;
using Component.Helpers;
using Component.Settings;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

// ReSharper disable IdentifierTypo
// ReSharper disable MemberCanBePrivate.Global

namespace Component.Extensions;

public static class ComponentServiceExtension
{
    public static void AddBaseComponentServiceExtension(this IServiceCollection sc, IConfiguration conf)
    {
        sc.AddComponentBaseController();
        sc.AddComponentAccessor();
        sc.AddComponentSettings(conf);
        sc.AddComponentMediatR();
        sc.AddComponentValidationBehavior();
        sc.AddConfigureApiBehaviorOptions();
        sc.AddMediaServiceClient();
    }

    public static void AddComponentBaseController(this IServiceCollection sc)
    {
        sc.AddControllers().AddNewtonsoftJson(options =>
        {
            options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            options.SerializerSettings.ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new DefaultNamingStrategy()
            };
        });
        sc.AddRouting(options => options.LowercaseUrls = true);
    }

    public static void AddComponentAccessor(this IServiceCollection sc)
    {
        sc.AddHttpContextAccessor();
        var sp = sc.BuildServiceProvider();
        UriHelper.Initialize(sp.GetRequiredService<IHttpContextAccessor>());
    }

    public static void AddComponentSettings(this IServiceCollection sc, IConfiguration conf)
    {
        sc.Configure<AppSetting>(conf.GetSection("AppSetting"));
        sc.Configure<MediaServiceSetting>(conf.GetSection("MediaServiceSetting"));
    }

    public static void AddComponentMediatR(this IServiceCollection sc) =>
        sc.AddMediatR(AppDomain.CurrentDomain.GetAssemblies());

    public static void AddComponentValidationBehavior(this IServiceCollection sc) =>
        sc.AddTransient(typeof(IPipelineBehavior<,>), typeof(BaseValidationBehavior<,>));


    public static void AddConfigureApiBehaviorOptions(this IServiceCollection sc) =>
        sc.Configure<ApiBehaviorOptions>(options => options.SuppressModelStateInvalidFilter = true);

    public static void AddMediaServiceClient(this IServiceCollection sc)
    { 
        var sp = sc.BuildServiceProvider();
        var mediaSetting = sp.GetRequiredService<IOptions<MediaServiceSetting>>().Value;

        sc.AddScoped<MediaServiceHandler>();
        sc.AddScoped<IMediaServiceModules, MediaServiceModules>();
        sc.AddHttpClient("MediaServiceClient", m => m.BaseAddress = new Uri(mediaSetting.Url))
            .AddHttpMessageHandler<MediaServiceHandler>();
    }
}