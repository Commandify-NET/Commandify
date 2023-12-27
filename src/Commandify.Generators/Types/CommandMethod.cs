using System.Collections.Immutable;

namespace Commandify.Generators.Types;

public readonly record struct CommandMethod(
    string Name,
    string MethodName,
    string? Description,
    string FullyQualifiedReturnType,
    ImmutableArray<CommandParameter> Parameters);