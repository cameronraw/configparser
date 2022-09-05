using ConfigParser.Entities;

namespace ConfigParser;

public class FileController
{
    private readonly string _filePath;

    public FileController(string filePath = "./TextFiles")
    {
        _filePath = filePath;
    }

    public Dictionary<ConfigFilePath, ConfigFileContents> GetConfigFiles()
    {
        var allFiles = Directory.GetFiles(_filePath);
        var configFiles = allFiles.Where(c => c.Contains("Config.txt"))
            .Select(c => new ConfigFilePath(c));

        var configFileContents = new Dictionary<ConfigFilePath, ConfigFileContents>();

        foreach (var configFile in configFiles)
        {
            try
            {
                var contentAsString = FetchConfigFileContents(configFile);
                var content = new ConfigFileContents(contentAsString);
                configFileContents.Add(configFile, content);
            }
            catch (IOException)
            {
                Console.WriteLine($"There was an error reading the file: {configFile}");
            }
        }

        return configFileContents;
    }

    private static string FetchConfigFileContents(ConfigFilePath configFile)
    {
        using var streamReader = new StreamReader(configFile.PathAsString);
        var contents = streamReader.ReadToEnd();
        return contents;
    }
}