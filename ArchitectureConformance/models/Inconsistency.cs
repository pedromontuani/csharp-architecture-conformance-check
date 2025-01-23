using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ArchitectureConformance.models;

public class Inconsistency(Entity origin, string reason)
{
    public Entity originEntity = origin;
    public string reason = reason;
}

public class InconsistencyEqualityComparer : IEqualityComparer<Inconsistency>
{
    public bool Equals(Inconsistency? x, Inconsistency y)
    {
        // Define your custom equality logic here
        return GetHashCode(x) == GetHashCode(y);
    }

    public int GetHashCode(Inconsistency obj)
    {
        // Define your custom hash code logic here
        return HashCode.Combine(obj.originEntity.classDeclaration.Identifier.Text, obj.reason);
    }
    
}