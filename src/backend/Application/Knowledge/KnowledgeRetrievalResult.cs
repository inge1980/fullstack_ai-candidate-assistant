namespace Application.Knowledge;

public sealed record KnowledgeRetrievalResult(
    IReadOnlyList<KnowledgeRetrievalItem> Items);

public sealed record KnowledgeRetrievalItem(
    string Source,
    string Heading,
    string SemanticType,
    string Content,
    Dictionary<string, object?> Metadata,
    double CombinedScore,
    double VectorScore,
    double MetadataScore,
    double EvidenceScore);