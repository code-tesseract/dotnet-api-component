namespace Component.Settings;

public class BaseDatabaseSetting
{
    public bool AutoMigrate { get; set; }
    public string? CollationType { get; set; } = "SQL_Latin1_General_CP1_CI_AS";
    public string InstanceName { get; set; } = null!;
    public string DatabaseName { get; set; } = null!;
    public bool IntegratedSecurity { get; set; } = true;
	public bool TrustServerCertificate { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
}