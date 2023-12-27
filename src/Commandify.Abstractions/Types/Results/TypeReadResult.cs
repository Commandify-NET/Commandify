namespace Commandify.Abstractions.Types.Results;

public readonly record struct TypeReadResult(
    bool Success,
    object Value);