namespace Commandify.Abstractions;

public readonly record struct CommandInvocation(
    bool Matched,
    CommandDelegate Delegate
);