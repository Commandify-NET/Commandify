using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Commandify.Generators.Types;

public readonly record struct CommandModule(
    Guid Id,
    string Name,
    string? Description,
    string ClassName,
    string Namespace,
    Guid? ParentModuleId,
    ImmutableArray<CommandMethod> Commands,
    ITypeSymbol Symbol);