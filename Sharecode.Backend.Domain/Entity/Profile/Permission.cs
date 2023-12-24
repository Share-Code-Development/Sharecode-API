using System.Reflection;
using Newtonsoft.Json;

namespace Sharecode.Backend.Domain.Entity.Profile;

[JsonConverter(typeof(PermissionConverter))]
public readonly struct Permission
{
    public string Key { get; init; }
    public string Description { get; init; }
    public bool IsAdminOnly { get; init; }

    internal Permission(string key, string description, bool isAdminOnly = false)
    {
        Key = key;
        Description = description;
        IsAdminOnly = isAdminOnly;
    }

    public static bool operator ==(Permission a, Permission b)
    {
        return a.Key == b.Key;
    }

    public static bool operator !=(Permission a, Permission b)
    {
        return a.Key != b.Key;
    }
    
    public static implicit operator Permission(string key)
    {
        return PermissionRepository.GetByKey(key) ?? throw new InvalidCastException($"No Permission found with key: {key}");
    }
    
    public static implicit operator string(Permission permission)
    {
        return permission.Key;
    }
    
    public static bool operator ==(string key, Permission permission)
    {
        return key == permission.Key;
    }

    public static bool operator !=(string key, Permission permission)
    {
        return key != permission.Key;
    }

    public override bool Equals(object? obj)
    {
        return obj is Permission permission && Key == permission.Key;
    }

    public override string ToString()
    {
        return Key;
    }
    
    public override int GetHashCode()
    {
        return Key.GetHashCode();
    }
}

public class PermissionConverter : JsonConverter
{
    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null)
        {
            return null; 
        }

        string? key = reader?.Value?.ToString() ?? null;
        if (key == null)
            return null;
        
        return PermissionRepository.GetByKey(key);
    }

    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(Permission);
    }
    
    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        if (value == null)
        {
            writer.WriteNull();
        }
        else
        {
            var permission = (Permission)value;
            writer.WriteValue(permission.Key);
        }
    }
}

public static class Permissions
{
    #region Snippet
        public static Permission CreateSnippet => new("create-snippet", "Create snippets");
        public static Permission ViewSnippet => new("view-snippet", "View snippets");
        public static Permission UpdateSnippet => new("update-snippet", "Update owned snippets");
        public static Permission DeleteSnippet => new("delete-snippet", "Delete owned snippets");

        #region Admin Permissions
        public static Permission ViewSnippetOthersAdmin => new("view-snippet-others-admin", "View others snippet even if its private as an admin", true);
        public static Permission UpdateSnippetOthers => new("update-snippet-others-admin", "Update others snippets", true);
        public static Permission DeleteSnippetOthers => new("delete-snippet-others-admin", "Delete others snippets", true);

        #endregion
        
    #endregion"

    #region User
    public static Permission UpdateUser => new("update-user", "Update the users settings");
    public static Permission ViewUserOtherMinimal => new("view-user-profile-others", "View minimal user information of others");

    #region Admin

    public static Permission ViewUserOtherAdmin => new("view-user-profile-others-admin", "View entire user information of others", true);

    #endregion

    #endregion
}

internal static class PermissionRepository
{
    private static readonly Dictionary<string, Permission> Permissions = new Dictionary<string, Permission>();

    static PermissionRepository()
    {
        {
            var permissionsClassProps = typeof(Permissions).GetProperties(BindingFlags.Static | BindingFlags.Public);

            foreach (var prop in permissionsClassProps)
            {
                if (prop.PropertyType == typeof(Permission))
                {
                    Permission permission = (Permission)prop.GetValue(null);
                    Permissions.Add(permission.Key, permission);
                }
            }
        }
    }

    public static Permission? GetByKey(string key)
    {
        var hasValue = Permissions.TryGetValue(key, out var perm);
        return hasValue ? perm : null;
    }

    public static ICollection<Permission> GetAdminOnlyPermissions()
    {
        return Permissions.Values
            .Where(p => p.IsAdminOnly)
            .ToHashSet();
    }
    
    public static ICollection<Permission> GetUserPermissions()
    {
        return Permissions.Values
            .Where(p => !p.IsAdminOnly)
            .ToHashSet();
    }

    public static ICollection<Permission> GetAll()
    {
        return Permissions.Values.ToHashSet();
    }
}