using Infrastructure.Documents;

namespace Infrastructure.Embeddings;

public class SearchResult
{
    public DocumentChunk Chunk { get; set; } = default!;

    public double VectorScore { get; set; }

    public double MetadataScore { get; set; }

    public double EvidenceScore { get; set; }

    public double CombinedScore { get; set; }
}