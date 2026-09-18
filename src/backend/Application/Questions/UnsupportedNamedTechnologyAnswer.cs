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

        var relatedSlugs = relatedItems
            .SelectMany(TechnologyCatalog.ProjectSlugs)
            .Where(slug => !named.Contains(slug, StringComparer.OrdinalIgnoreCase))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(slug => slug, StringComparer.OrdinalIgnoreCase)
            .Select(TechnologyCatalog.DisplayName)
            .ToList();

        var projectTitles = relatedItems
            .Select(AnswerPromptFormatter.ProjectTitle)
            .Where(title => !string.IsNullOrWhiteSpace(title))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (QuestionLocale.Normalize(locale) == QuestionLocale.Nb)
        {
            var answer =
                $"Jeg har ikke brukt {namedText}. Det står ikke i Technologies-feltet på noen av de indekserte prosjektene, og jeg finner ikke prosjektbelegg for at jeg har brukt det.";

            if (relatedSlugs.Count > 0 && projectTitles.Count > 0)
            {
                answer +=
                    $" Jeg har relatert erfaring med {JoinNatural(relatedSlugs, locale)} i {JoinNatural(projectTitles, locale)}. Det er ikke {namedText}-erfaring.";
            }

            return answer;
        }

        var english =
            $"I have not used {namedText}. It is not listed in any project's Technologies field, and there is no project evidence that I used it.";

        if (relatedSlugs.Count > 0 && projectTitles.Count > 0)
        {
            english +=
                $" I do have related experience with {JoinNatural(relatedSlugs, locale)} in {JoinNatural(projectTitles, locale)}. That is not {namedText} experience.";
        }

        return english;
    }

    public static string DebugPrompt(
        string? question,
        string locale,
        IReadOnlyList<KnowledgeRetrievalItem> relatedItems)
    {
        var named = TechnologyCatalog.ResolveNamed(question);
        return
            "[Grounded refusal - LLM skipped]\n"
            + $"Named technologies with no exact Technologies match: {string.Join(", ", named)}\n"
            + $"Related family projects: {string.Join(", ", relatedItems.Select(AnswerPromptFormatter.ProjectTitle))}\n\n"
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
