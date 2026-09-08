namespace Application.Questions;

public sealed record AskQuestionRequest(
    string Question,
    string? Locale = null
);