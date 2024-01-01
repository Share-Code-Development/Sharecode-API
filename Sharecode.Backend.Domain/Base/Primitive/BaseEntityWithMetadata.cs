using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using Sharecode.Backend.Utilities.MetaKeys;

namespace Sharecode.Backend.Domain.Base.Primitive;

public abstract class BaseEntityWithMetadata : BaseEntity
{
    public Dictionary<string, object> Metadata { get; set; } = new();
    
    public bool SetMeta(MetaKey key, object value)
    {
        if (value.GetType() != key.ValueType)
        {
            throw new InvalidOperationException(
                $"The provided value is of type {value.GetType().Name} while the required type is of {key.ValueType.Name}");
        }
        
        Metadata[key.Key] = value;
        return true;
    }

    /// <summary>
    /// Sets the metadata value for the given key.
    /// </summary>
    /// <param name="key">The key of the metadata.</param>
    /// <param name="value">The value to set. Pass null to remove the metadata.</param>
    /// <returns>The previous value associated with the key if it exists, otherwise null.</returns>
    public object? SetMeta(string key, object? value)
    {
        // Check that key is not null, empty, or whitespace
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Key cannot be null, empty, or whitespace.", nameof(key));
        
        // Early return if the value is null
        if (value == null)
        {
            if (Metadata.Remove(key, out var oldValue))
            {
                return oldValue;
            }

            return null;
        }

        // Assign new value and return old value (if existed)
        if (Metadata.TryGetValue(key, out var oldValue2))
        {
            Metadata[key] = value;
            return oldValue2;
        }

        // No previous value, just set new value
        Metadata[key] = value;
        return null;
    }

    
    public T? ReadMeta<T>(MetaKey key, T? defaultValue = default)
    {
        if (Metadata.TryGetValue(key.Key, out var value))
        {
            try
            {
                return value is T value1 ? value1 : default;
            }
            catch (JsonException)
            {
                throw new InvalidOperationException(
                    $"Failed to get the metadata of {key}. Current value is {value} and expected type is {key.ValueType.Name}");
            }
        }

        return defaultValue;
    }

    public void DeleteMeta(MetaKey key)
    {
        Metadata.Remove(key.Key);
    }
    
    
    
}
