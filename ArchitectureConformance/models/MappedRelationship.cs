namespace ArchitectureConformance.models;

public class MappedRelationship
{
    public string namespaceRegex { get; set; }
    public List<string> mustReference { get; set; } = new();
    public List<string> cantReference { get; set; } = new();
    public List<string> whitelistedEntities { get; set; } = new();
}