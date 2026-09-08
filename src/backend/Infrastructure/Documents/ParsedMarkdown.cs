namespace Infrastructure.Documents;

public class ParsedMarkdown
{
    public string Content { get; set; } = "";

    public Dictionary<string, object?> Metadata { get; set; } = new();
}