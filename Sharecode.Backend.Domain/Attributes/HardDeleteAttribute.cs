namespace Sharecode.Backend.Domain.Attributes;

/// <summary>
/// Specifies that an entity should be hard deleted.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class HardDeleteAttribute : Attribute
{
    // You can add more properties or methods here if needed
}