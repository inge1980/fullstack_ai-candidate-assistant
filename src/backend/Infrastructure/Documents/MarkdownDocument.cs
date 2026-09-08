namespace Infrastructure.Documents;

public class MarkdownDocument
{
    public string FileName { get; set; } = "";

    public string Content { get; set; } = "";

    public Dictionary<string, object?> Metadata { get; set; } = new();
}