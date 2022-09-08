using System.ComponentModel;
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
            returnedConfigValue = GetConfigValueFromFileContents(config, configId);
        }

        if (string.IsNullOrEmpty(returnedConfigValue))
        {
            throw new Exception("Config value was not found.");
        }

        return TryParseTypeFromString<T>(returnedConfigValue);
    }


    private List<ConfigFileContents> PrioritizeConfigFileContents
        (Dictionary<ConfigFilePath, ConfigFileContents> configFiles)
    {
        var validConfigFileSuffixes = new List<string>
        {
            "base",
            "project",
            "experiment"
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
            if (!line.Contains(configId)) continue;
            return line.Split("\t").Last();
        }

        return string.Empty;
    }

    private static T TryParseTypeFromString<T>(string rawStringFromConfig)
    {
        var converter = TypeDescriptor.GetConverter(typeof(T));

        var convertedValue = (T)converter.ConvertFromString(rawStringFromConfig)!;

        if (convertedValue == null)
        {
            throw new Exception($"Value could not be converted to {typeof(T)}");
        }

        return convertedValue;
    }
}