using System.Collections.Immutable;
using System.Reflection;

namespace Commandify.Abstractions.Types.Results;

public readonly record struct ArgumentsParseResult(
    bool Success,
    ImmutableArray<object> Arguments,
    ImmutableArray<ParameterInfo> UnparsedParameters);