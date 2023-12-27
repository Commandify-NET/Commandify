using System.Collections.Immutable;
using System.Reflection;

namespace Commandify.Abstractions.Types;

public readonly record struct CommandInfo(string Name, bool IsDefaultCommand, MethodInfo Method)
{
    public ImmutableArray<ParameterInfo> Parameters => Method.GetParameters().ToImmutableArray();
}