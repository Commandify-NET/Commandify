using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Commandify.Generators;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class CommandMethodAnalyzer : DiagnosticAnalyzer
{
    public readonly DiagnosticDescriptor ShouldBeAsyncDiagnostic = new("COM001", "Command method should be async", "Command method should be async", "Commandify", DiagnosticSeverity.Error, true);
    public readonly DiagnosticDescriptor SameMethodNameDiagnostic = new("COM002", "Command method should be different", "Command method should be different", "Commandify", DiagnosticSeverity.Error, true);
    
    public override void Initialize(AnalysisContext context)
    {
        context.RegisterSyntaxNodeAction(ctx =>
        {
            var classDeclaration = ctx.Node as ClassDeclarationSyntax;

            if (!classDeclaration.BaseList.Types.Any(_ => _.ToString() == "ICommandModule"))
                return;
            
            var allCommandMethods = classDeclaration.Members.OfType<MethodDeclarationSyntax>()
                .Select(_ => (ctx.SemanticModel.GetDeclaredSymbol(_), _))
                .ToList();

            foreach (var (method, node) in allCommandMethods)
            {
                if (allCommandMethods.Count(_ => _.Item1.Name == method.Name) > 1)
                {
                    ctx.ReportDiagnostic(Diagnostic.Create(SameMethodNameDiagnostic, node.Identifier.GetLocation()));
                }
            }

        }, SyntaxKind.ClassDeclaration);
        
        context.RegisterSyntaxNodeAction(ctx =>
        {
            var method = ctx.Node as MethodDeclarationSyntax;

            var methodSymbol = ctx.SemanticModel.GetDeclaredSymbol(method);
            
            bool isCommand = methodSymbol.GetAttributes().Any(_ => _.AttributeClass.Name == "CommandAttribute");
            
            if (isCommand && methodSymbol.ReturnType.ToString() != "System.Threading.Tasks.Task")
            {
                ctx.ReportDiagnostic(Diagnostic.Create(ShouldBeAsyncDiagnostic, method.ReturnType.GetLocation()));
            }

        }, SyntaxKind.MethodDeclaration);
    }

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray<DiagnosticDescriptor>.Empty
        .Add(ShouldBeAsyncDiagnostic)
        .Add(SameMethodNameDiagnostic);
}