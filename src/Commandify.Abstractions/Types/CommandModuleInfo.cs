using System.Collections.Immutable;

namespace Commandify.Abstractions.Types;

public readonly record struct CommandModuleInfo(string Name, Type Type, ImmutableArray<CommandInfo> Commands, ImmutableArray<CommandModuleInfo>? ChildModules = null!);