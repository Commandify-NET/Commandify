using System.Collections.Immutable;
using System.Reflection;
using Commandify.Abstractions.Conversion;
using Commandify.Abstractions.Types.Results;
using Commandify.Extensions;

namespace Commandify.Execution;

public static class CommandArgumentsParser
{
    public static ArgumentsParseResult Parse(ReadOnlySpan<char> arguments, ImmutableArray<ParameterInfo> parameters, ITypeReaderPipeline typeReaderPipeline)
    {
        if (parameters.IsEmpty)
            return new ArgumentsParseResult(true, ImmutableArray<object>.Empty, ImmutableArray<ParameterInfo>.Empty);
        
        var enumerator = arguments.Split();

        Span<object> argumentValues = new object[parameters.Length];
        
        bool allParametersSet = false;
        
        Span<ParameterInfo> unparsedParameters = new ParameterInfo[parameters.Length];
        int unparsedParametersCount = 0;
        
        for (int i = 0; i < parameters.Length && enumerator.MoveNext(); i++)
        {
            var segment = enumerator.Current;
            var parameter = parameters[i];

            if (segment == ReadOnlySpan<char>.Empty)
            {
                if (parameter.IsOptional)
                {
                    unparsedParameters[unparsedParametersCount++]  = parameter;
                    
                    continue;
                }

                allParametersSet = false;
                break;
            }

            var result = typeReaderPipeline.Read(segment, parameter.ParameterType);

            if (result.Success)
            {
                argumentValues[i] = result.Value;
            }
            else if (!parameter.IsOptional)
            {
                unparsedParameters[unparsedParametersCount++] = parameter;
                
                allParametersSet = false;
                break;
            }
            
            allParametersSet = parameters.Length - 1 == i;
        }

        return new ArgumentsParseResult(allParametersSet, argumentValues.ToImmutableArray(),
            unparsedParameters.Slice(0, unparsedParametersCount).ToImmutableArray());
    }
}