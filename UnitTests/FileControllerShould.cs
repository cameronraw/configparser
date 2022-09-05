using FluentAssertions;

namespace ConfigParser;

public class FileControllerShould
{
    [Test]
    public void DetectFiles_WithConfigInTheName()
    {
        var fileController = new FileController("TestTextFiles");
        IEnumerable<string> configFiles = fileController.GetConfigFiles();

        configFiles.Should().HaveCount(1);
    }
}