namespace Application.Questions;

public sealed record QuestionSource(
    string ProjectId,
    string Title,
    string? Url,
    string? Heading,
    string? SemanticType,
    string? Content,
    string? Source,
    QuestionRelevance? Relevance
);