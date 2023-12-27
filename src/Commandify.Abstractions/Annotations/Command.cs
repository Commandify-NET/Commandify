namespace Commandify.Abstractions.Annotations;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class CommandModuleAttribute : Attribute
{
    public required string Name { get; init; }
    
    public string? Description { get; init; }
}

// [AttributeUsage(AttributeTargets.Class)]
// public class ParentModuleAttribute<TParent> : Attribute
//     where TParent : ICommandModule
// {
//     
// }

[AttributeUsage(AttributeTargets.Method)]
public class CommandAttribute : Attribute
{
    public string? Name { get; init; }
    
    public string? Description { get; init; }
}