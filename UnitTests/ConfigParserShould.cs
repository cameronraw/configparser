using ConfigParser.Entities;
using FluentAssertions;
using Moq;

namespace ConfigParser;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void ReturnASpecifiedLoadedConfigValue()
    {
        var mockedConfigContents = "- Order Profile:\nordersPerHour:\t5000\norderLinesPerOrder:\t10";
        
        var fileController = new Mock<FileController.IFileController>();
        var mockedConfigFiles = new Dictionary<ConfigFilePath, ConfigFileContents>
        {
            {new ConfigFilePath("MockedConfigContents"), new ConfigFileContents(mockedConfigContents)}
        };
        fileController.Setup(mock => mock.GetConfigFiles())
            .Returns(mockedConfigFiles);
        
        var configParser = new ConfigParser(fileController.Object);

        var ordersPerHour = configParser.GetConfig<int>(mockedConfigFiles.First().Value, 
            "ordersPerHour");

        ordersPerHour.Should().Be(5000);
    }

    [Test]
    public void ThrowAnException_IfTypeIsNotValid()
    {
        var mockedConfigContents = "- Order Profile:\nordersPerHour:\tnotanint\norderLinesPerOrder:\t10";
        
        var fileController = new Mock<FileController.IFileController>();
        var mockedConfigFiles = new Dictionary<ConfigFilePath, ConfigFileContents>
        {
            {new ConfigFilePath("MockedConfigContents"), new ConfigFileContents(mockedConfigContents)}
        };
        fileController.Setup(mock => mock.GetConfigFiles())
            .Returns(mockedConfigFiles);
        
        var configParser = new ConfigParser(fileController.Object);

        Action attemptToGetConfig = () => configParser.GetConfig<int>(mockedConfigFiles.First().Value,
            "ordersPerHour");

        attemptToGetConfig.Should().Throw<Exception>();
    }
}