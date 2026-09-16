namespace Application.Questions;

public enum QuestionPhase
{
    Translating,
    Searching,
    Writing,
    TryingAnotherModel
}

public sealed record QuestionProgress(
    QuestionPhase Phase,
    string? Provider = null,
    string? Model = null);

public static class QuestionPhaseCodes
{
    public const string Translating = "translating";
    public const string Searching = "searching";
    public const string Writing = "writing";
    public const string TryingAnotherModel = "trying-another-model";

    public static string ToEventCode(this QuestionPhase phase)
    {
        return phase switch
        {
            QuestionPhase.Translating => Translating,
            QuestionPhase.Searching => Searching,
            QuestionPhase.Writing => Writing,
            QuestionPhase.TryingAnotherModel => TryingAnotherModel,
            _ => throw new ArgumentOutOfRangeException(nameof(phase), phase, null)
        };
    }
}
