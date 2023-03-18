namespace Component.Settings;

public class BaseDatabaseSetting
{
    public string? CollationType { get; set; } = "SQL_Latin1_General_CP1_CI_AS";
    public string InstanceName { get; set; } = null!;
    public string DatabaseName { get; set; } = null!;
    public bool IntegratedSecurity { get; set; } = true;
    public string? Username { get; set; } = null;
    public string? Password { get; set; } = null;
}