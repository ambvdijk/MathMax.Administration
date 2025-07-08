using System;

namespace MathMax.EventSourcing.Core.Repositories;

/// <summary>
/// Exception thrown when an entity is not found in the repository.
/// </summary>
public class EntityNotFoundException : Exception
{
    public EntityNotFoundException() : base()
    {
    }

    public EntityNotFoundException(string message) : base(message)
    {
    }

    public EntityNotFoundException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    /// Creates an EntityNotFoundException with a formatted message for a specific entity type and ID.
    /// </summary>
    /// <param name="entityId">The ID of the entity that was not found.</param>
    /// <returns>A new EntityNotFoundException with a descriptive message.</returns>
    public static EntityNotFoundException ForEntity<T>(object entityId)
    {
        return ForEntity(typeof(T).Name, entityId);
    }

    /// <summary>
    /// Creates an EntityNotFoundException with a formatted message for a specific entity type and ID.
    /// </summary>
    /// <param name="entityTypeName">The name of the entity type that was not found.</param>
    /// <param name="entityId">The ID of the entity that was not found.</param>
    /// <returns>A new EntityNotFoundException with a descriptive message.</returns>
    public static EntityNotFoundException ForEntity(string entityTypeName, object entityId)
    {
        return new EntityNotFoundException($"Entity of type '{entityTypeName}' with ID '{entityId}' was not found.");
    }
}
