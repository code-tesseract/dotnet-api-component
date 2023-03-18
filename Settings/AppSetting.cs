using Microsoft.Extensions.Hosting;

namespace Component.Settings;

public class AppSetting
{
    public string AppName { get; set; } = null!;
    public string AppUrl { get; set; } = null!;
    public string? AppEnv { get; set; } = Environments.Development;
    public AppClientSetting AppClientSetting { get; set; } = null!;
}