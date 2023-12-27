namespace Commandify.Abstractions;

public delegate Task CommandDelegate(IServiceProvider serviceProvider, CancellationToken cancellationToken = default!);