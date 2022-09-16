using System.Collections.Generic;
using ConfigParser.Entities;

namespace ConfigParser.Interfaces;

public interface IFileController
{
    public Dictionary<ConfigFilePath, ConfigFileContents> GetConfigFiles();
}