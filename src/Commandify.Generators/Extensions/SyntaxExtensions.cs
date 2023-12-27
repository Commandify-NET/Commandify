using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Commandify.Generators.Extensions;

public static partial class SyntaxExtensions
{
    public static string? GetCommandGroupAttributeValue(this ClassDeclarationSyntax classDeclarationSyntax, SemanticModel semanticModel, string name)
    {
        return classDeclarationSyntax.GetAttributeArgumentValue(semanticModel, "CommandModuleAttribute", name) is
            { HasValue: true, Value: { } constValue }
            ? constValue.Value?.ToString() : null;
    }
    
    public static string? GetCommandAttributeValue(this MethodDeclarationSyntax methodDeclarationSyntax, SemanticModel semanticModel, string name)
    {
        return methodDeclarationSyntax.GetAttributeArgumentValue(semanticModel, "CommandAttribute", name) is
            { HasValue: true, Value: { } constValue }
            ? constValue.Value?.ToString() : null;
    }
    
    
    
    
    
    public static Optional<TypedConstant> GetAttributeArgumentValue(this MemberDeclarationSyntax syntax,
        SemanticModel model,
        string attributeName,
        string argumentName)
    {
        var symbol = model.GetDeclaredSymbol(syntax);

        foreach (var attribute in symbol.GetAttributes())
        {
            if (!string.Equals(attributeName, attribute.AttributeClass.Name, StringComparison.OrdinalIgnoreCase))
                continue;

            foreach (var namedArgument in attribute.NamedArguments)
            {
                if (string.Equals(namedArgument.Key, argumentName, StringComparison.OrdinalIgnoreCase))
                {
                    return namedArgument.Value;
                }
            }
        }
        
        return new Optional<TypedConstant>();
    }
    
    public static Optional<ImmutableArray<TypedConstant>> GetAttributeArgumentValues(this MemberDeclarationSyntax syntax,
        SemanticModel model,
        string attributeName,
        string argumentName)
    {
        var symbol = model.GetDeclaredSymbol(syntax);

        foreach (var attribute in symbol.GetAttributes())
        {
            if (!string.Equals(attributeName, attribute.AttributeClass.Name, StringComparison.OrdinalIgnoreCase))
                continue;

            foreach (var namedArgument in attribute.NamedArguments)
            {
                if (string.Equals(namedArgument.Key, argumentName, StringComparison.OrdinalIgnoreCase))
                {
                    return namedArgument.Value.Values;
                }
            }
        }
        
        return new Optional<ImmutableArray<TypedConstant>>();
    }
}