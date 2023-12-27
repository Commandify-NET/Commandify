using Commandify.Abstractions.Types;

namespace Commandify.Abstractions;

public interface ICommandModule
{
    static abstract string Name { get; }
    
    static abstract CommandModuleInfo ModuleInfo { get; }
}