namespace ConfigParser;

public class ConfigValueNotFoundException : Exception
{
    public ConfigValueNotFoundException(string message) : base(message) {}
}