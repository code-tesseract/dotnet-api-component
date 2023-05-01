using Component.Helpers;
using Component.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace Component.Extensions;

public static class ConfigureBuilderExtension
{
	public static void AddConfigureWebHostBuilder(this WebApplicationBuilder appBuilder, AppSetting appSetting)
	{
		appBuilder.WebHost
				  .UseUrls(appSetting.AppUrl.Split(';'))
				  .ConfigureKestrel(options => options.Limits.MaxRequestBodySize = 10_485_760)
				  .UseIISIntegration();
	}

	public static IConfigurationRoot AddConfigurationRoot(
		this IConfigurationBuilder configurationBuilder, string configRootPath)
		=> configurationBuilder.AddJsonFiles(configRootPath).AddEnvironmentVariables().Build();

	public static IConfigurationBuilder AddJsonFiles(
		this IConfigurationBuilder configurationBuilder, string configRootPath)
	{
		if (!Directory.Exists(configRootPath)) throw new Exception("Config root path directory not found.");
		var configJsonList = DirectoryHelper.GetFileNameList(configRootPath, "json");
		configurationBuilder.SetBasePath(configRootPath);
		configJsonList.ForEach(fn => configurationBuilder.AddJsonFile(fn, optional: true, reloadOnChange: true));
		return configurationBuilder;
	}

	public static AppSetting GetAppSettings(this IConfiguration configuration)
		=> configuration.GetSection(nameof(AppSetting)).Get<AppSetting>();
}