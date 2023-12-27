using Commandify.Abstractions.Types.Results;

namespace Commandify.Abstractions.Conversion;

public interface ITypeReaderPipeline
{
    TypeReadResult Read(ReadOnlySpan<char> input, Type type);
}