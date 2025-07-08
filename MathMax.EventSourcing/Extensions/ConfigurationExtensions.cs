using System;
using Microsoft.Extensions.Configuration;

namespace MathMax.EventSourcing.Extensions;

public static class ConfigurationExtensions
{
    /// <summary>
    /// Gets the configuration value for the specified key and throws an exception if not found.
    /// This method guarantees a non-null return value, satisfying null-safety requirements.
    /// </summary>
    /// <param name="configuration">The configuration instance</param>
    /// <param name="key">The configuration key</param>
    /// <returns>The configuration value as a non-null string</returns>
    /// <exception cref="ConfigurationException">Thrown when the configuration key is not found or is null/empty</exception>
    public static string GetRequiredValue(this IConfiguration configuration, string key)
    {
        var value = configuration[key];
        
        if (string.IsNullOrEmpty(value))
        {
            throw new ConfigurationException(key);
        }
        
        return value;
    }

    /// <summary>
    /// Gets the configuration value for the specified key with a fallback default value.
    /// This method guarantees a non-null return value.
    /// </summary>
    /// <param name="configuration">The configuration instance</param>
    /// <param name="key">The configuration key</param>
    /// <param name="defaultValue">The default value to return if the key is not found</param>
    /// <returns>The configuration value or default value as a non-null string</returns>
    public static string GetValueOrDefault(this IConfiguration configuration, string key, string defaultValue)
    {
        var value = configuration[key];
        return string.IsNullOrEmpty(value) ? defaultValue : value;
    }
}
