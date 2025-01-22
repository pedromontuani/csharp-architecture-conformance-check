using System.Collections.Concurrent;

namespace ArchitectureConformance.models;

public class Pkg()
{
    public MappedRelationship mappedRelationship;
    
    public ConcurrentBag<Entity> entities = [];

    public ConcurrentBag<Inconsistency> inconsistencies = [];

    public ConcurrentBag<string> absences = [];

    public Pkg(MappedRelationship mappedRelationship): this()
    {
        this.mappedRelationship = mappedRelationship;
    }
}