using System.Text.RegularExpressions;

namespace Application.Questions;

public static class QuestionIntentDetector
{
    public const string Detail = "detail";
    public const string List = "list";
    public const string Count = "count";
    public const string FilterList = "filter-list";

    private static readonly Regex RequestedCountRegex = new(
        @"\btopp?\s+(\d+)|\b(?:list|liste)\s+(?:the\s+)?(?:topp?\s+)?(\d+)",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);

    private static readonly Regex CountRegex = new(
        @"\bhow\s+many\b|\bhvor\s+mange\b|\bnumber\s+of\s+projects?\b|\bantall\s+(?:prosjekter|prosjekt)\b",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);

    private static readonly Regex ListRegex = new(
        @"\blist\b|\bliste\b|\btopp?\s+\d+|\brank(?:ed|ing)?\b|\branger(?:e|ing)?\b|\bwhich\s+projects?\b|\bhvilke\s+prosjekter\b",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);

    private static readonly Regex CatalogCueRegex = new(
        @"\bprojects\b|\bprosjekter\b|\bresults?\b|\bresultater\b|\btopp?\s+\d+",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);

    private static readonly Regex SectionInventoryRegex = new(
        @"\btrade-?offs?\b|\bchallenges?\b|\bimplement(?:ation|ed|ing)?\b|\barchitecture\b|\blessons?\b|\btechnical\s+decisions?\b|\bavveininger\b|\butfordringer\b|\barkitektur\b",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);

    private static readonly Regex TimeFilterRegex = new(
        @"\b(?:last|past)\s+\d+\s+years?\b|\bsiste\s+\d+\s+år(?:ene)?\b|\bsince\s+\d{4}\b|\bde\s+siste\s+\d+\s+år(?:ene)?\b",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);

    private static readonly Regex TopicFilterRegex = new(
        @"\brelevant\s+to\b|\bin\s+regards?\s+to\b|\bthat\s+handles?\b|\bwith\s+(?:regard|respect)\s+to\b|\bi\s+forhold\s+til\b|\bsom\s+håndterer\b",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);

    private static readonly Regex TechnologyFilterRegex = new(
        @"\b(?:sql|php|postgresql|postgres|mysql|sqlite|react|typescript|javascript|csharp|dotnet|aspnet|python|docker|azure|n8n|rag|llm|ollama|groq|redis|graphql|nextjs|next\.js)\b|\bai\b",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);

    public static QuestionIntent Detect(string? question)
    {
        if (string.IsNullOrWhiteSpace(question))
        {
            return new QuestionIntent(Detail, null);
        }

        var text = question.Trim();
        var requestedCount = ExtractRequestedCount(text);
        var isCount = CountRegex.IsMatch(text);
        var isList = ListRegex.IsMatch(text) || requestedCount is not null;
        var hasFilter = HasFilterLanguage(text);

        if (isList && IsSectionInventory(text))
        {
            return new QuestionIntent(Detail, requestedCount);
        }

        if ((isCount || isList) && hasFilter)
        {
            return new QuestionIntent(FilterList, requestedCount);
        }

        if (isCount)
        {
            return new QuestionIntent(Count, requestedCount);
        }

        if (isList)
        {
            return new QuestionIntent(List, requestedCount);
        }

        return new QuestionIntent(Detail, requestedCount);
    }

    private static int? ExtractRequestedCount(string text)
    {
        var match = RequestedCountRegex.Match(text);
        if (!match.Success)
        {
            return null;
        }

        for (var groupIndex = 1; groupIndex < match.Groups.Count; groupIndex++)
        {
            var group = match.Groups[groupIndex];
            if (group.Success && int.TryParse(group.Value, out var count) && count > 0)
            {
                return count;
            }
        }

        return null;
    }

    private static bool IsSectionInventory(string text)
    {
        return SectionInventoryRegex.IsMatch(text)
            && !CatalogCueRegex.IsMatch(text);
    }

    private static bool HasFilterLanguage(string text)
    {
        return TimeFilterRegex.IsMatch(text)
            || TopicFilterRegex.IsMatch(text)
            || TechnologyFilterRegex.IsMatch(text);
    }
}
