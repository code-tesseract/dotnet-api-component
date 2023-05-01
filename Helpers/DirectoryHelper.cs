using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Component.Helpers;

public static class DirectoryHelper
{
	private static IWebHostEnvironment? _env;

	public static void ConfigureDirectoryHelper(this IServiceProvider serviceProvider)
		=> _env = serviceProvider.GetRequiredService<IWebHostEnvironment>();

	public static string? WebRootPath => _env?.WebRootPath;

	public static List<string> GetFileNameList(string targetDirectory, string fileFormat)
		=> new DirectoryInfo(targetDirectory)
		   .GetFiles($"*.{fileFormat}")
		   .Select(fileInfo => fileInfo.Name)
		   .ToList();
}