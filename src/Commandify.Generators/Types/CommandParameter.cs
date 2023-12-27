namespace Commandify.Generators.Types;

public readonly record struct CommandParameter(
    string Name,
    string FullyQualifiedType);