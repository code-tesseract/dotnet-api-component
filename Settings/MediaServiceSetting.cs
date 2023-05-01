namespace Component.Settings;

public class MediaServiceSetting
{
	public bool   Enable       { get; set; }
	public string Url          { get; set; } = null!;
	public string PartnerId    { get; set; } = null!;
	public string ClientKey    { get; set; } = null!;
	public string ClientSecret { get; set; } = null!;
}