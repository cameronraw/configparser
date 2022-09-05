namespace ConfigParser;

public class FileController
{
    private readonly string _filePath;

    public FileController(string filePath = "./TextFiles")
    {
        _filePath = filePath;
    }

    public IEnumerable<string> GetConfigFiles()
    {
        var currentDirectory = Directory.GetCurrentDirectory();
        var directoryPrefix = Directory.GetParent(currentDirectory)!.Parent!.Parent!.FullName;
        
        var allFiles = Directory.GetFiles($"{directoryPrefix}/{_filePath}");
        return allFiles.Where(c => c.Contains("Config.txt"));
    }
}