﻿using Component.Base;
using Component.Helpers;
using Component.Middlewares;
using Component.Settings;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
        sc.AddComponentBaseDbContext();
        sc.AddResponseMiddleware();
        sc.AddComponentMediatR();
        sc.AddComponentValidationBehavior();
        sc.AddComponentValidatorsFromAssembly();
        sc.AddConfigureApiBehaviorOptions();
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
        sc.Configure<BaseDatabaseSetting>(conf.GetSection("BaseDatabaseSetting"));
        sc.Configure<MediaSetting>(conf.GetSection("MediaSetting"));
    }

    public static void AddComponentBaseDbContext(this IServiceCollection sc) => sc.AddDbContext<BaseDbContext>();

    public static void AddResponseMiddleware(this IServiceCollection sc) => sc.AddTransient<ResponseMiddleware>();

    public static void AddComponentMediatR(this IServiceCollection sc) =>
        sc.AddMediatR(typeof(AssemblyReference).Assembly);

    public static void AddComponentValidationBehavior(this IServiceCollection sc) =>
        sc.AddTransient(typeof(IPipelineBehavior<,>), typeof(BaseValidationBehavior<,>));

    public static void AddComponentValidatorsFromAssembly(this IServiceCollection sc) =>
        sc.AddValidatorsFromAssembly(typeof(AssemblyReference).Assembly);

    public static void AddConfigureApiBehaviorOptions(this IServiceCollection sc) =>
        sc.Configure<ApiBehaviorOptions>(options => options.SuppressModelStateInvalidFilter = true);
}