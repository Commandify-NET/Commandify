using System.Collections.Immutable;
using Commandify.Generators.Helpers;
using Commandify.Generators.Types;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Commandify.Generators;

[Generator]
public class CommandsGenerator : IIncrementalGenerator
{
    public const string CommandModuleAttribute = "Commandify.Abstractions.Annotations.CommandModuleAttribute";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // if (!Debugger.IsAttached)
        //     Debugger.Launch();
        
        var provider = context.SyntaxProvider
            .ForAttributeWithMetadataName(CommandModuleAttribute, IsCommandModule, (_, t) => _)
            .Collect()
            .Combine(context.CompilationProvider)
            .Select(Transformer.Transform);
        
        context.RegisterSourceOutput(provider, Generate);
    }

    private bool IsCommandModule(SyntaxNode node, CancellationToken cancellationToken)
    {
        if (node is not ClassDeclarationSyntax classDeclarationSyntax)
            return false;

        if (!classDeclarationSyntax.Modifiers.Any(_ => _.IsKind(SyntaxKind.PartialKeyword)))
            return false;

        return true;
    }
    
    private void Generate(SourceProductionContext context, (SemanticModel Model, ImmutableArray<CommandModule> Modules, Compilation Compilation) data)
    {
        foreach (var module in data.Modules)
        {
            string partialClass = Generator.GeneratePartialClass(data.Model, module);
            
            context.AddSource($"{module.ClassName}.g.cs", partialClass);
        }

        // var executorCode = Generator.GenerateExecutor(data.Model, data.Modules, data.Compilation.AssemblyName!);
        //
        // context.AddSource("CommandExecutor.g.cs", executorCode);
    }
}