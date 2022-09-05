namespace ConfigParser;

public class FileController
{
    private readonly string _filePath;

    public FileController(string filePath = "./TextFiles")
    {
        _filePath = filePath;
    }

    public Dictionary<string, string> GetConfigFiles()
    {
        var allFiles = Directory.GetFiles(_filePath);
        var configFiles = allFiles.Where(c => c.Contains("Config.txt"));

        var configFileContents = new Dictionary<string, string>();

        foreach (var configFile in configFiles)
        {
            try
            {
                var contents = FetchConfigFileContents(configFile);
                configFileContents.Add(configFile, contents);
            }
            catch (IOException)
            {
                Console.WriteLine($"There was an error reading the file: {configFile}");
            }
        }

        return configFileContents;
    }

    private static string FetchConfigFileContents(string configFile)
    {
        using var streamReader = new StreamReader(configFile);
        var contents = streamReader.ReadToEnd();
        return contents;
    }
}