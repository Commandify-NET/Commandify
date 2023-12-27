using Commandify.Abstractions.Types.Results;

namespace Commandify.Abstractions.Conversion.TypeReaders;

public interface ITypeReader
{
    bool IsSupported(Type type);

    TypeReadResult Read(ReadOnlySpan<char> input, Type type);
}