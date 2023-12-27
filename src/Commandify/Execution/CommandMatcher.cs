using Commandify.Abstractions.Types;
using Commandify.Enumerators;
using Commandify.Extensions;
using Commandify.Types.Results;

namespace Commandify.Execution;

public static class CommandMatcher
{
    public static CommandMatchResult GetMatch(
        ReadOnlySpan<char> text,
        ReadOnlyMemory<CommandModuleInfo> modules)
    {
        var segmentEnumerable = text.Split();
        
        return GetMatchingModule(segmentEnumerable, modules);

        CommandMatchResult GetMatchingModule(ReadOnlySpanSplitEnumerator enumerator, ReadOnlyMemory<CommandModuleInfo> modules)
        {
            if (!enumerator.MoveNext())
            {
                return new CommandMatchResult();
            }
            
            var segment = enumerator.Current;
            
            foreach (var module in modules.Span)
            {
                if (!segment.Equals(module.Name, StringComparison.OrdinalIgnoreCase))
                    continue;

                CommandMatchResult childMatched = new CommandMatchResult();
                
                if (module.ChildModules is { IsDefaultOrEmpty: false } childModules)
                {
                    childMatched = GetMatchingModule(enumerator, childModules.AsMemory());
                }

                if (childMatched.Matched)
                {
                    return childMatched;
                }

                //bool nextSegmentAvailable = enumerator.MoveNext();

                CommandInfo? matchedDefaultCommand = null;

                bool hasDefaultCommand = false;

                foreach (var defaultCommand in module.Commands.Where(_ => _.IsDefaultCommand))
                {
                    var parametersCount = defaultCommand.Parameters.Length;

                    int i = parametersCount;
                    
                    bool hasEnoughArguments = i == 0;
                    
                    while (i-- > 0)
                    {
                        if (enumerator.MoveNext())
                        {
                            hasEnoughArguments = true;
                        }
                        else hasEnoughArguments = false;
                    }

                    while (parametersCount-- > 0)
                    {
                        enumerator.MoveBack();
                    }

                    if (hasEnoughArguments)
                    {
                        matchedDefaultCommand = defaultCommand;
                        hasDefaultCommand = true;
                        break;
                    }
                }

                bool matchingCommands = false;

                if (matchingCommands = enumerator.MoveNext())
                {
                    segment = enumerator.Current;
                
                    foreach (var command in module.Commands)
                    {
                        if (!command.IsDefaultCommand && segment.Equals(command.Name, StringComparison.OrdinalIgnoreCase))
                        {
                            return new CommandMatchResult(module, command, enumerator.AsSpan());
                        }
                    }
                }

                if (hasDefaultCommand)
                {
                    if(matchingCommands)
                        enumerator.MoveBack();
                    return new CommandMatchResult(module, matchedDefaultCommand.Value, enumerator.AsSpan());
                }
            }

            return new CommandMatchResult();
        }
    }
}