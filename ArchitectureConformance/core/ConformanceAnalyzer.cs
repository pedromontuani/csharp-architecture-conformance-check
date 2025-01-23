using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using ArchitectureConformance.models;

namespace ArchitectureConformance.core;

public class ConformanceAnalyzer(ConcurrentDictionary<string, Pkg> systemArchitecture)
{
    
    public void Analyze()
    {
        var entities = systemArchitecture.SelectMany(pkg => pkg.Value.entities);
        Parallel.ForEach(entities, CheckForForbiddenReferences);
        Parallel.ForEach(systemArchitecture.Values, CheckForAbsentRelationships);
        RemoveDuplications();
    }

    public List<Pkg> GetProcessedPackages()
    {
        return systemArchitecture.Values.ToList();
    }
    
    private void CheckForForbiddenReferences(Entity entity)
    {
        if(IsEntityWhitelisted(entity)) return;
        
        var pkg = entity.parentPkg;
        var mappedRelationship = pkg.mappedRelationship;
        var cantReference = mappedRelationship.cantReference;
        cantReference.ForEach(namespaceRegex =>
        {
            entity.referencedNamespaces.ToList().ForEach(rNamespace =>
            {
                if (Regex.IsMatch(rNamespace, namespaceRegex))
                {
                    pkg.inconsistencies.Add(new Inconsistency(entity, rNamespace));
                }
            });
            
        });
    }

    private void CheckForAbsentRelationships(Pkg pkg)
    {
        var mappedRelationship = pkg.mappedRelationship;
        var requiredReferences = mappedRelationship.mustReference;
        var pkgEntities = pkg.entities.ToList();
        
        requiredReferences.ForEach(namespaceRegex =>
        {
            var referencedEntities = pkgEntities.SelectMany(p => p.referencedNamespaces).ToList();
            var includesNamespace = referencedEntities.Any(r => Regex.IsMatch(r, namespaceRegex));
           
            if (!includesNamespace)
            {
                pkg.absences.Add(namespaceRegex);
            }
        });
        
    }

    private bool IsEntityWhitelisted(Entity entity)
    {
        var excludedEntities = entity.parentPkg.mappedRelationship.excludedEntities;
        var entityPkgName = SyntaxAnalyzer.GetPackageName(entity.classDeclaration);
        var fullEntityName = $"{entityPkgName}.{entity.classDeclaration.Identifier.Text}";
        
        return excludedEntities.Any(excluded => Regex.IsMatch(fullEntityName, excluded));
    }
    
    private void RemoveDuplications()
    {
        Parallel.ForEach(systemArchitecture.Values, pkg =>
        {
            pkg.inconsistencies = new ConcurrentBag<Inconsistency>(pkg.inconsistencies.Distinct(new InconsistencyEqualityComparer()));
            pkg.absences = new ConcurrentBag<string>(pkg.absences.Distinct());
        });

    }
}