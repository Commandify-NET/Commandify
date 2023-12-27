using Commandify.Abstractions.Conversion;
using Commandify.Abstractions.Conversion.TypeReaders;
using Microsoft.Extensions.DependencyInjection;

namespace Commandify.Abstractions.Builders;

public interface ITypeReaderPipelineBuilder
{
    ITypeReaderPipelineBuilder UseReader<TReader>(ServiceLifetime serviceLifetime = ServiceLifetime.Singleton) where TReader : ITypeReader;

    ITypeReaderPipeline Build(IServiceProvider serviceProvider);
}