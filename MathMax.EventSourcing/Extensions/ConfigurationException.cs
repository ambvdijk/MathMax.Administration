using System;

namespace MathMax.EventSourcing.Extensions;

/// <summary>
/// Exception thrown when a required configuration value is missing or invalid.
/// </summary>
public class ConfigurationException : Exception
{
    public string ConfigurationKey { get; }

    public ConfigurationException(string configurationKey)
        : base($"Configuration key '{configurationKey}' is required but was not found or is empty.")
    {
        ConfigurationKey = configurationKey;
    }

    public ConfigurationException(string configurationKey, string message)
        : base(message)
    {
        ConfigurationKey = configurationKey;
    }

    public ConfigurationException(string configurationKey, string message, Exception innerException)
        : base(message, innerException)
    {
        ConfigurationKey = configurationKey;
    }
    
    public static ConfigurationException RequiredValue(string key)
    {
        return new ConfigurationException(key, $"Configuration key '{key}' is required but was not found or is empty.");
    }
}
