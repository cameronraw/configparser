namespace ConfigParser.Entities;

public class ConfigFileContents
{
    public string ContentsAsString { get; }

    public ConfigFileContents(string contentsAsString)
    {
        ContentsAsString = contentsAsString;
    }
}