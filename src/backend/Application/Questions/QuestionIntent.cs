namespace Application.Questions;

public sealed record QuestionIntent(
    string Category,
    int? RequestedCount);
