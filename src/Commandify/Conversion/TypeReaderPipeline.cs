using System.Collections.Immutable;
using Commandify.Abstractions.Conversion;
using Commandify.Abstractions.Conversion.TypeReaders;
using Commandify.Abstractions.Types.Results;

namespace Commandify.Conversion;

public class TypeReaderPipeline : ITypeReaderPipeline
{
    private ImmutableArray<ITypeReader> _readers;
    
    public TypeReaderPipeline(ImmutableArray<ITypeReader> typeReaders)
    {
        _readers = typeReaders;
    }
    
    public TypeReadResult Read(ReadOnlySpan<char> input, Type type)
    {
        foreach (var reader in _readers)
        {
            if (reader.IsSupported(type))
            {
                try
                {
                    var result = reader.Read(input, type);

                    if (result is { Success: true, Value: {} value })
                    {
                        return result;
                    }
                }
                catch
                {
                    return new TypeReadResult(false, default!);
                }
            }
        }
        
        return new TypeReadResult(false, null!);
    }
}