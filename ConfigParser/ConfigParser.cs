using System.ComponentModel;
using System.Text.RegularExpressions;
using ConfigParser.Entities;
using ConfigParser.Interfaces;

namespace ConfigParser;

public class ConfigParser
{
    private readonly IFileController _fileController;
    private Dictionary<ConfigFilePath, ConfigFileContents> _loadedConfigFiles;

    public ConfigParser(IFileController fileController)
    {
        _fileController = fileController;
        _loadedConfigFiles = new Dictionary<ConfigFilePath, ConfigFileContents>();
    }

    public void LoadConfigs()
    {
        _loadedConfigFiles = _fileController.GetConfigFiles();
    }

    public T GetConfig<T>(string configId)
    {
        var returnedConfigValue = string.Empty;

        var orderedConfigOptions = PrioritizeConfigFileContents(_loadedConfigFiles);

        foreach (var config in orderedConfigOptions)
        {
            var configValueFromFileContents = GetConfigValueFromFileContents(config, configId);
            if (configValueFromFileContents == string.Empty) continue;
            returnedConfigValue = configValueFromFileContents;
        }

        if (string.IsNullOrEmpty(returnedConfigValue))
        {
            throw new ConfigValueNotFoundException($"Config value {configId} was not found.");
        }

        return TryParseTypeFromString<T>(returnedConfigValue);
    }


    private List<ConfigFileContents> PrioritizeConfigFileContents
        (Dictionary<ConfigFilePath, ConfigFileContents> configFiles)
    {
        var validConfigFileSuffixes = new List<string>
        {
            ConfigSuffix.Base.ToLowerCaseString(),
            ConfigSuffix.Project.ToLowerCaseString(),
            ConfigSuffix.Experiment.ToLowerCaseString()
        };

        List<ConfigFileContents> orderedConfigs = validConfigFileSuffixes
            .Select(CheckForSuffixInFileNames)
            .Where(config => config != null).ToList()!;

        if (orderedConfigs.Count != 0) return orderedConfigs;

        Console.WriteLine("Config file naming convention not followed, be aware of unexpected behaviour.");
        return configFiles.Select(c => c.Value).ToList();
    }

    private ConfigFileContents? CheckForSuffixInFileNames(string suffix)
    {
        foreach (var configFile in _loadedConfigFiles)
        {
            var configFilePath = configFile.Key.PathAsString.ToLower();

            if (configFilePath.Contains(suffix))
            {
                return configFile.Value;
            }
        }
        return null;
    }


    private static string GetConfigValueFromFileContents(ConfigFileContents configFileContents, string configId)
    {
        var configAsString = configFileContents.ContentsAsString;
        var lines = configAsString.Split("\n");

        foreach (var line in lines)
        {
            var cleanedLine = RemoveComments(line);
            
            if (!cleanedLine.Contains(configId)) continue;
            return cleanedLine.Split('\u0009'.ToString(), StringSplitOptions.RemoveEmptyEntries)[1];
        }

        return string.Empty;
    }

    private static string RemoveComments(string commentedString)
    {
        var commentPosition = commentedString.IndexOf("//", StringComparison.Ordinal);
        if(commentPosition == -1)
        {
            return commentedString;
        }

        return commentedString.Remove(commentPosition);
    }

    private static T TryParseTypeFromString<T>(string rawStringFromConfig)
    {
        var converter = TypeDescriptor.GetConverter(typeof(T));

        return (T)converter.ConvertFromString(rawStringFromConfig)!;
    }
}