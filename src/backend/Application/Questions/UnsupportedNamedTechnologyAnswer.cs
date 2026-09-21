using Application.Knowledge;

namespace Application.Questions;

public static class UnsupportedNamedTechnologyAnswer
{
    public static string Format(
        string? question,
        string locale,
        IReadOnlyList<KnowledgeRetrievalItem> relatedItems)
    {
        var named = TechnologyCatalog.ResolveNamed(question);
        var namedText = JoinNatural(
            named.Select(TechnologyCatalog.DisplayName).ToList(),
            locale);

        var relatedNames = TechnologyCatalog
            .RelatedFamilySlugsPresent(question, relatedItems)
            .Select(TechnologyCatalog.DisplayName)
            .ToList();

        if (QuestionLocale.Normalize(locale) == QuestionLocale.Nb)
        {
            var answer = $"Jeg har ikke brukt {namedText}.";
            if (relatedNames.Count > 0)
            {
                answer += $" Jeg har brukt {JoinNatural(relatedNames, locale)}.";
            }

            return answer;
        }

        var english = $"I have not used {namedText}.";
        if (relatedNames.Count > 0)
        {
            english += $" I have used {JoinNatural(relatedNames, locale)}.";
        }

        return english;
    }

    public static string DebugPrompt(
        string? question,
        string locale,
        IReadOnlyList<KnowledgeRetrievalItem> relatedItems)
    {
        var named = TechnologyCatalog.ResolveNamed(question);
        var relatedProjects = PromptContextSelector.RelatedFamilyContext(
            question,
            relatedItems);

        return
            "[Grounded refusal - LLM skipped]\n"
            + $"Named technologies with no exact Technologies match: {string.Join(", ", named)}\n"
            + $"Related family technologies: {string.Join(", ", TechnologyCatalog.RelatedFamilySlugsPresent(question, relatedItems))}\n"
            + $"Related family projects: {string.Join(", ", relatedProjects.Select(AnswerPromptFormatter.ProjectTitle))}\n\n"
            + Format(question, locale, relatedItems);
    }

    private static string JoinNatural(IReadOnlyList<string> items, string locale)
    {
        if (items.Count == 0)
        {
            return string.Empty;
        }

        if (items.Count == 1)
        {
            return items[0];
        }

        var conjunction = QuestionLocale.Normalize(locale) == QuestionLocale.Nb
            ? "og"
            : "and";

        if (items.Count == 2)
        {
            return $"{items[0]} {conjunction} {items[1]}";
        }

        return string.Join(", ", items.Take(items.Count - 1))
            + $", {conjunction} {items[^1]}";
    }
}
