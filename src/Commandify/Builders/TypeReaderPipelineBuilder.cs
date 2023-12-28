using System.Collections.Immutable;
using Commandify.Abstractions.Builders;
using Commandify.Abstractions.Conversion;
using Commandify.Abstractions.Conversion.TypeReaders;
using Commandify.Conversion;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Commandify.Builders;

public class TypeReaderPipelineBuilder : ITypeReaderPipelineBuilder
{
    private readonly IServiceCollection _serviceCollection;
    private ImmutableArray<Func<IServiceProvider, ITypeReader>> _readers;

    public TypeReaderPipelineBuilder(IServiceCollection serviceCollection)
    {
        _serviceCollection = serviceCollection;
        _readers = ImmutableArray<Func<IServiceProvider, ITypeReader>>.Empty;
    }
    
    public ITypeReaderPipelineBuilder UseReader<TReader>(ServiceLifetime serviceLifetime = ServiceLifetime.Singleton) where TReader : ITypeReader
    {
        _serviceCollection.TryAdd(ServiceDescriptor.Describe(typeof(TReader), typeof(TReader), serviceLifetime));
        
        return new TypeReaderPipelineBuilder(_serviceCollection) { _readers = _readers.Add(sp => sp.GetRequiredService<TReader>()) };
    }
    
    public ITypeReaderPipelineBuilder UseReader(ITypeReader typeReader)
    {
        return new TypeReaderPipelineBuilder(_serviceCollection) { _readers = _readers.Add(sp => typeReader) };
    }

    public ITypeReaderPipeline Build(IServiceProvider serviceProvider)
    {
        ImmutableArray<ITypeReader> readers = _readers.Select(x => x(serviceProvider)).ToImmutableArray();

        return new TypeReaderPipeline(readers);
    }
}