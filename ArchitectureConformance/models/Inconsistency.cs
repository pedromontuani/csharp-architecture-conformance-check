using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ArchitectureConformance.models;

public class Inconsistency(Entity origin, string reason)
{
    public Entity originEntity = origin;
    public string reason = reason;
}