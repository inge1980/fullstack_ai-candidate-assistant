using Pgvector;

namespace Infrastructure.Documents;

public class DocumentChunk
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public string Source { get; set; } = string.Empty;

    public string HeadingPath { get; set; } = string.Empty;

    public string SemanticType { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public Dictionary<string, object?> Metadata { get; set; } = new();

    public Vector Embedding { get; set; } = default!;
}