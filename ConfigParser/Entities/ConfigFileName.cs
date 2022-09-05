namespace ConfigParser.Entities;

public class ConfigFilePath
{
    public ConfigFilePath(string path)
    {
        PathAsString = path;
    }

    public string PathAsString { get; }
}