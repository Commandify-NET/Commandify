using System.Text;
using Commandify.Generators.Types;
using Microsoft.CodeAnalysis;

namespace Commandify.Generators.Helpers;

public partial class Generator
{
    public const string DefaultNamespace = "Commandify.Generated";

    public static string GeneratePartialClass(
        SemanticModel semanticModel,
        CommandModule module)
    {
        List<ITypeSymbol> types = new List<ITypeSymbol>();

        ITypeSymbol currentType = module.Symbol;

        do
        {
            types.Add(currentType);

            currentType = currentType.ContainingType;
        } while (currentType != null);

        types.Reverse();

        StringBuilder sb = new StringBuilder();

        sb.AppendLine($"namespace {module.Namespace};");
        sb.AppendLine($"using Commandify.Abstractions;");
        sb.AppendLine($"using Commandify.Abstractions.Execution;");
        sb.AppendLine($"using Commandify.Abstractions.Types;");
        sb.AppendLine($"using Commandify.Execution;");
        sb.AppendLine($"using System.Collections.Immutable;");

        int padding = 0;

        foreach (var type in types)
        {
            var isLast = SymbolEqualityComparer.Default.Equals(types.Last(), type);

            string paddingS = new string(' ', padding * 4);

            sb.Append($@"

{paddingS}public partial class {type.Name} : ICommandModule
{paddingS}{{
");
            if (isLast)
            {
                string commandsArray = !module.Commands.IsEmpty
                    ? $"ImmutableArray.Create({string.Join(", ", module.Commands.Select(_ => @$"new CommandInfo(""{_.Name}"", {(module.Name == _.Name ? "true" : "false")}, typeof({module.ClassName}).GetMethod(""{_.MethodName}""))"))})"
                    : "ImmutableArray<CommandInfo>.Empty";
                
                sb.Append($@"
{paddingS}    public static string Name => ""{module.Name}"";
{paddingS}    public static CommandModuleInfo ModuleInfo => new CommandModuleInfo(Name, typeof({module.ClassName}), {commandsArray});
");
            }


            padding++;
        }

        for (int i = 0; i < padding; i++)
        {
            sb.AppendLine($"{new string(' ', (padding - 1) * 4)}}}");
        }

        return sb.ToString();
    }
}