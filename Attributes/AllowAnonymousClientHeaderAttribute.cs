namespace Component.Attributes;

// ReSharper disable ClassNeverInstantiated.Global
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AllowAnonymousClientHeaderAttribute : Attribute
{
}