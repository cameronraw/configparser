using ConfigParser.Entities;
using ConfigParser.Interfaces;
using FluentAssertions;
using Moq;

namespace ConfigParser;

public class Tests
{
    private const string MockedValidConfigContents = "- Order Profile:\nordersPerHour:\t5000\norderLinesPerOrder:\t10";

    private const string MockedInvalidConfigContents =
        "- Order Profile:\nordersPerHour:\tnotanint\norderLinesPerOrder:\t10";

    private Mock<IFileController> _fileController = null!;

    [SetUp]
    public void Setup()
    {
        _fileController = new Mock<IFileController>();
    }

    [Test]
    public void ReturnASpecifiedLoadedConfigValue()
    {
        var mockedConfigFiles = new Dictionary<ConfigFilePath, ConfigFileContents>
        {
            { new ConfigFilePath("MockedConfigContents"), new ConfigFileContents(MockedValidConfigContents) }
        };

        _fileController.Setup(mock => mock.GetConfigFiles())
            .Returns(mockedConfigFiles);

        var configParser = new ConfigParser(_fileController.Object);
        configParser.LoadConfigs();

        var ordersPerHour = configParser.GetConfig<int>("ordersPerHour");

        ordersPerHour.Should().Be(5000);
    }

    [Test]
    public void PrioritizeConfigsByFileName()
    {
        var baseConfigString = "- Order Profile:\nordersPerHour:\t5000\norderLinesPerOrder:\t10";
        var projectConfigString = "- Order Profile:\nordersPerHour:\t6000\norderLinesPerOrder:\t10";
        var experimentalConfigString = "- Order Profile:\nordersPerHour:\t7000\norderLinesPerOrder:\t10";
        
        var mockedConfigFiles = new Dictionary<ConfigFilePath, ConfigFileContents>
        {
            { new ConfigFilePath("ProjectConfig"), new ConfigFileContents(projectConfigString) },
            { new ConfigFilePath("ExperimentalConfig"), new ConfigFileContents(experimentalConfigString) },
            { new ConfigFilePath("BaseConfig"), new ConfigFileContents(baseConfigString) }
        };
        
        _fileController.Setup(mock => mock.GetConfigFiles())
            .Returns(mockedConfigFiles);

        var configParser = new ConfigParser(_fileController.Object);
        configParser.LoadConfigs();

        var ordersPerHour = configParser.GetConfig<int>("ordersPerHour");

        ordersPerHour.Should().Be(7000);
    }

    [Test]
    public void ThrowAnException_IfTypeIsNotValid()
    {
        var mockedConfigFiles = new Dictionary<ConfigFilePath, ConfigFileContents>
        {
            { new ConfigFilePath("MockedConfigContents"), new ConfigFileContents(MockedInvalidConfigContents) }
        };

        _fileController.Setup(mock => mock.GetConfigFiles())
            .Returns(mockedConfigFiles);

        var configParser = new ConfigParser(_fileController.Object);

        Action attemptToGetConfig = () => configParser.GetConfig<int>("ordersPerHour");

        attemptToGetConfig.Should().Throw<Exception>();
    }

    [Test]
    public void ThrowAnException_IfConfigNotFound()
    {
        var mockedConfigFiles = new Dictionary<ConfigFilePath, ConfigFileContents>
        {
            { new ConfigFilePath("MockedConfig1"), new ConfigFileContents(MockedValidConfigContents) },
            { new ConfigFilePath("MockedConfig2"), new ConfigFileContents(MockedValidConfigContents) },
            { new ConfigFilePath("MockedConfig3"), new ConfigFileContents(MockedValidConfigContents) }
        };

        _fileController.Setup(mock => mock.GetConfigFiles())
            .Returns(mockedConfigFiles);

        var configParser = new ConfigParser(_fileController.Object);

        Action attemptToGetConfig = () => configParser.GetConfig<int>("nonExistentConfigId");

        attemptToGetConfig.Should().Throw<Exception>();
    }
}