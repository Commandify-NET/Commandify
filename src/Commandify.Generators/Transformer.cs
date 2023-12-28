using System.Collections.Immutable;
using Commandify.Generators.Extensions;
using Commandify.Generators.Helpers;
using Commandify.Generators.Types;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Commandify.Generators;

public class Transformer
{
    public static (SemanticModel Model, ImmutableArray<CommandModule> Modules, Compilation Compilation) Transform(
        (ImmutableArray<GeneratorAttributeSyntaxContext> Left, Compilation Right) valueTuple,
        CancellationToken token)
    {
        var syntaxes = valueTuple.Left;
        
        ImmutableArray<CommandModule> modules = ImmutableArray<CommandModule>.Empty;

        SemanticModel? model = null;

        foreach (var syntax in syntaxes)
        {
            model ??= syntax.SemanticModel;

            modules = modules.Add(Transform(syntax));
        }

        foreach (var module in modules)
        {
            modules = modules.Replace(module, ParentLookup(module, modules));
        }

        return (model!, modules, valueTuple.Right);
    }

    private static CommandModule Transform(GeneratorAttributeSyntaxContext syntaxContext)
    {
        var moduleClass = (syntaxContext.TargetNode as ClassDeclarationSyntax)!;

        var moduleNamespace =
            syntaxContext.TargetSymbol.ContainingNamespace is { IsGlobalNamespace: false } namespaceSymbol
                ? namespaceSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat).Replace("global::", "")
                : Generator.DefaultNamespace;


        var moduleName = moduleClass.GetCommandGroupAttributeValue(syntaxContext.SemanticModel, "Name")
                         ?? syntaxContext.TargetSymbol.Name;

        var moduleDescription = moduleClass.GetCommandGroupAttributeValue(syntaxContext.SemanticModel, "Description")
                                ?? null;

        return new CommandModule
        {
            Id = Guid.NewGuid(),
            Name = moduleName,
            ClassName = moduleClass.Identifier.ToString(),
            Description = moduleDescription,
            Namespace = moduleNamespace,
            Commands = GetCommands(syntaxContext, moduleName, moduleDescription).ToImmutableArray(),
            Symbol = syntaxContext.TargetSymbol as ITypeSymbol
        };
    }


    private static IEnumerable<CommandMethod> GetCommands(GeneratorAttributeSyntaxContext syntaxContext,
        string groupName, string? groupDescription)
    {
        var methods = syntaxContext.TargetNode.ChildNodes()
            .OfType<MethodDeclarationSyntax>();

        foreach (var method in methods)
        {
            var methodSymbol = syntaxContext.SemanticModel.GetDeclaredSymbol(method) as IMethodSymbol;

            if (methodSymbol?.ReturnType is not { Name: "Task" })
                continue;
            
            if(!methodSymbol.GetAttributes().Any(_ => _.AttributeClass?.Name is "CommandAttribute"))
                continue;

            var commandName = method.GetCommandAttributeValue(syntaxContext.SemanticModel, "Name") ??
                              groupName ?? methodSymbol.Name;

            var commandDescription = method.GetCommandAttributeValue(syntaxContext.SemanticModel, "Description") ??
                                     groupDescription;

            yield return new CommandMethod
            {
                Name = commandName,
                Description = commandDescription,
                MethodName = methodSymbol.Name,
                FullyQualifiedReturnType =
                    methodSymbol.ReturnType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                Parameters = GetParameters().ToImmutableArray()
            };

            IEnumerable<CommandParameter> GetParameters()
            {
                var parameters = methodSymbol.Parameters;

                foreach (var parameter in parameters)
                {
                    yield return new CommandParameter
                    {
                        Name = parameter.Name,
                        FullyQualifiedType = parameter.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
                    };
                }
            }
        }
    }


    private static CommandModule ParentLookup(CommandModule module, ImmutableArray<CommandModule> allModules)
    {
        var currentModule = module;

        if (module.Symbol.ContainingType?.GetAttributes().Any(_ => _.AttributeClass?.Name is "CommandModuleAttribute") ?? false)
        {
            var parentModule = allModules.Single(_ => _.ClassName == currentModule.Symbol.ContainingType.Name);

            currentModule = currentModule with
            {
                ParentModuleId = parentModule.Id
            };
        }

        else if (module.Symbol.GetAttributes().SingleOrDefault(_ =>
                     _.AttributeClass?.Name is "ParentModuleAttribute")?.AttributeClass.TypeArguments.Single() is
                 { Name: { } parentModuleName })
        {
            var parentModule = allModules.Single(_ => _.ClassName == parentModuleName);

            currentModule = currentModule with
            {
                ParentModuleId = parentModule.Id
            };
        }

        return currentModule;
    }
}