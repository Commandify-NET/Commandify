using Microsoft.CodeAnalysis;

namespace Commandify.Generators.Extensions;

public static partial class SyntaxExtensions
{
    public static string GetFullyQualifiedName(this SyntaxNode node, SemanticModel model)
    {
        var symbol = model.GetDeclaredSymbol(node);

        if (symbol is null)
        {
            throw new InvalidOperationException("Could not get symbol for node.");
        }

        return symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
    }
}