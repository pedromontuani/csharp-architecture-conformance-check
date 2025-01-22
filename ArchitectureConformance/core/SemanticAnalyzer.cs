using ArchitetureConformance.utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ArchitectureConformance.core;

public static class SemanticAnalyzer
{
    public static async Task<List<ClassDeclarationSyntax>> GetReferencedClasses(ClassDeclarationSyntax classDeclarationSyntax, Project msProject)
    {
        var semanticModel = await GetSemanticModel(classDeclarationSyntax, msProject);
        
        var referencedClasses = SyntaxAnalyzer.GetReferencedClasses(classDeclarationSyntax, semanticModel);
        
        var classDeclarations = referencedClasses.Select(ClassDeclarationSyntaxHelper.GetClassDeclarationSyntax)
            .ToArray();

        var filteredTasks = classDeclarations.Where(c => c != null)
            .Select(async c => new { Class = c, IsPresent = await ProjectUtils.IsClassPresentAsync(msProject, c.Identifier.Text) });

        var filteredResults = await Task.WhenAll(filteredTasks);
        
        return filteredResults.Where(result => result.IsPresent)
            .Select(result => result.Class).Distinct().ToList()!;
    }
    
    private static async Task<SemanticModel> GetSemanticModel(ClassDeclarationSyntax classDeclarationSyntax, Project msProject)
    {
        var classTree = classDeclarationSyntax.SyntaxTree;
        var references = await MetadataReferenceHelper.GetMetadataReferencesAsync(msProject);
        var compilation = CSharpCompilation.Create("MyCompilation", new[] { classTree }, references);
        var semanticModel = compilation.GetSemanticModel(classTree);

        return semanticModel;
    }

}