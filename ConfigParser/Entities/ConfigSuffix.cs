namespace ConfigParser.Entities;

public enum ConfigSuffix
{
    Base,
    Project,
    Experiment
}

public static class ConfigSuffixExtensions
{
    public static string ToLowerCaseString(this ConfigSuffix suffixEnum)
    {
        return suffixEnum switch
        {
            ConfigSuffix.Base => "base",
            ConfigSuffix.Experiment => "experiment",
            ConfigSuffix.Project => "project",
            _ => ""
        };
    }
}