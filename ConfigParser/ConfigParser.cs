using System.ComponentModel;
using ConfigParser.Entities;

namespace ConfigParser;

public class ConfigParser
{
    private FileController.IFileController _fileController;

    public ConfigParser(FileController.IFileController fileController)
    {
        _fileController = fileController;
    }

    public T GetConfig<T>(ConfigFileContents contents, string configId) 
    {
        var configFileContents = contents.ContentsAsString;
        var lines = configFileContents.Split("\n");
        var rawConfigValue = String.Empty;
        
        foreach (var line in lines)
        {
            if (!line.Contains(configId)) continue;
            rawConfigValue = line.Split("\t").Last();
            break;
        }

        var converter = TypeDescriptor.GetConverter(typeof(T));

        var convertedValue = (T)converter.ConvertFromString(rawConfigValue)!;

        if (convertedValue == null)
        {
            throw new Exception($"Value could not be converted to {typeof(T)}");
        }

        return convertedValue;
    }
}