namespace Application.Questions;

public static class QuestionLocale
{
    public const string Us = "us";
    public const string Nb = "nb";

    public static string Normalize(string? locale)
    {
        return string.Equals(locale, Nb, StringComparison.OrdinalIgnoreCase)
            ? Nb
            : Us;
    }

    public static bool RequiresQueryTranslation(string locale)
    {
        return locale == Nb;
    }

    public static string AnswerLanguageInstruction(string locale)
    {
        if (locale == Nb)
        {
            return
                """
                Write the entire answer in fluent Norwegian Bokmål (nb).
                Sound like a native speaker: natural word order, idioms, and tone.
                Do not produce literal, word-for-word translation from English.
                Keep technology names, product names, and proper nouns as they appear in the evidence.
                The retrieved context is English; that is expected. Still answer in Norwegian.
                """;
        }

        return "Write the entire answer in fluent American English.";
    }
}
