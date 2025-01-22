using System.Collections.Concurrent;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ArchitectureConformance.models;

public class Entity
{
    public ClassDeclarationSyntax classDeclaration;
    public Pkg parentPkg;
    public ConcurrentBag<string> referencedNamespaces = [];
    
    public Entity(ClassDeclarationSyntax classDeclaration, Pkg parentPkg)
    {
        this.classDeclaration = classDeclaration;
        this.parentPkg = parentPkg;
    }
   
}