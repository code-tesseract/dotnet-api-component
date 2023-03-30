namespace Component.Externals.MediaService;

public static class MediaServiceEndpoints
{
    public static (HttpMethod m, string p) Requirements => (HttpMethod.Get, "requirements");
    public static (HttpMethod m, string p) Upload => (HttpMethod.Post, "upload");
    public static (HttpMethod m, string p) Uploads => (HttpMethod.Post, "uploads");
    public static (HttpMethod m, string p) UploadBase64 => (HttpMethod.Post, "upload-base64");
}