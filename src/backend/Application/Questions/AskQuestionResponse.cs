namespace Application.Questions;

public sealed record AskQuestionResponse(
    string Answer,
    IReadOnlyList<QuestionSource> Sources
);