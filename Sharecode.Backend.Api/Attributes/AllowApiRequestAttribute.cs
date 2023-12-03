namespace Sharecode.Backend.Api.Attributes;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public class AllowApiRequestAttribute : Attribute
{
}