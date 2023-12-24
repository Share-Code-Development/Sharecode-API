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
