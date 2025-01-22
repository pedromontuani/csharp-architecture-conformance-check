using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using ArchitectureConformance.models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Pkg = ArchitectureConformance.models.Pkg;

namespace ArchitectureConformance.core;

public class Analyzer(Project msProject, List<MappedRelationship> mappedArchitecture)
{
    private readonly ConcurrentDictionary<string, Pkg> _systemArchitecture = new ();
    private List<ClassDeclarationSyntax> _projectClasses = [];
    
    public async Task Analyze()
    {
        _projectClasses = (await GetProjectClasses()).ToList();
        MapRelationships();
        GetEntitiesRelationships();
    }
    
    public ConcurrentDictionary<string, Pkg> GetSystemArchitecture()
    {
        return _systemArchitecture;
    }

    private async Task<List<ClassDeclarationSyntax>> GetProjectClasses()
    {
        var tasks = msProject.Documents.Select(async d => new CsFile(d,
            (await d.GetSyntaxTreeAsync()))).ToArray();
        
        var projectFiles = await Task.WhenAll(tasks);

        return projectFiles.SelectMany(p => p.classDeclarations).ToList();
    }

    private void MapRelationships()
    {
        mappedArchitecture.ForEach(mapped =>
        {
            var pkg = new Pkg(mapped);
            _systemArchitecture.TryAdd(mapped.namespaceRegex, pkg);
        });
        
        Parallel.ForEach(_projectClasses,  (classDeclaration) =>
        {
            var classNamespace = SyntaxAnalyzer.GetPackageName(classDeclaration);
            var matchedNamespaces = _systemArchitecture.Keys.Where(k => Regex.IsMatch(classNamespace, k)).ToList();

            if (matchedNamespaces.Count == 0) return;
            
            var bestMatch = GetBestNamespaceMatch(matchedNamespaces);
            var pkg = _systemArchitecture[bestMatch];
            var entity = new Entity(classDeclaration, pkg);
                
            _systemArchitecture[bestMatch].entities.Add(entity);
        });
    }

    private void GetEntitiesRelationships()
    {
        var entities = _systemArchitecture.Values.SelectMany(p => p.entities).ToList();

        Parallel.ForEach(entities,  (entity, _) =>
        {
            var referencedNamespaces = SyntaxAnalyzer.GetReferencedNamespaces(entity.classDeclaration);
            
            referencedNamespaces.ForEach(entity.referencedNamespaces.Add);
        });
        
    }

    private string GetBestNamespaceMatch(List<string> namespacesRegexList)
    {
        if (namespacesRegexList.Count == 1)
        {
            return namespacesRegexList.First();
        }
        
        return namespacesRegexList.OrderByDescending(n => n.Length).First();
    }
    
}
