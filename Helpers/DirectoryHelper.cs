namespace Component.Helpers;

public static class DirectoryHelper
{
    public static string Current => Directory.GetCurrentDirectory();

    public static string GetParentPath(string path) => Path.Combine($"{Directory.GetParent(Current)}", path);

    public static List<string> GetFileNameList(string targetDirectory, string fileFormat)
    {
        var fileInfos = new DirectoryInfo(targetDirectory).GetFiles($"*.{fileFormat}");
        return fileInfos.Select(fileInfo => fileInfo.Name).ToList();
    }
}