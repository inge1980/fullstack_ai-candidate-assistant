namespace Application.Questions;

public sealed record QuestionDebugInfo(
    IReadOnlyList<RetrievedSourceDebug> RetrievedSources
);

public sealed record RetrievedSourceDebug(
    string SourceFile,
    string? Heading,
    string? SemanticType,
    double CombinedScore,
    double VectorScore,
    double MetadataScore,
    double EvidenceScore
);