using System.ComponentModel;
using ConfigParser.Entities;

namespace ConfigParser;

public class ConfigParser
{
    private FileController.IFileController _fileController;
    private List<ConfigFileContents> _orderedConfigOptions;

    public ConfigParser(FileController.IFileController fileController)
    {
        _fileController = fileController;
        var rawConfigFiles = _fileController.GetConfigFiles();
        _orderedConfigOptions = PrioritizeConfigFileContents(rawConfigFiles);
    }

    public T GetConfig<T>(string configId)
    {
        var returnedConfigValue = string.Empty;

        foreach (var config in _orderedConfigOptions)
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
        var orderedConfigs = new List<ConfigFileContents>();

        var validConfigFileSuffixes = new List<string>
        {
            "base",
            "project",
            "experiment"
        };

        foreach (var suffix in validConfigFileSuffixes)
        {
            foreach (var configFile in configFiles)
            {
                var configFilePath = configFile.Key.PathAsString.ToLower();

                if (configFilePath.Contains(suffix))
                {
                    orderedConfigs.Add(configFile.Value);
                }
            }
        }

        if (orderedConfigs.Count != 0) return orderedConfigs;

        Console.WriteLine("Config file naming convention not followed, be aware of unexpected behaviour.");
        return configFiles.Select(c => c.Value).ToList();
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