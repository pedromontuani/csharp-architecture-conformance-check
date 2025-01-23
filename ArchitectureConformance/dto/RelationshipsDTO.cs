namespace ArchitetureConformance.dto;

public class RelationshipsDTO
{
    public string namespaceRegex { get; set; }
    public List<string> mustReference { get; set; } = [];
    public List<string> divergences { get; set; } = [];
    public List<string> absences { get; set; } = [];
}