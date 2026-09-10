using System.Text.RegularExpressions;
using Application.Knowledge;

namespace Application.Questions;

public static class PromptContextSelector
{
    public const int DefaultRetrievalLimit = 25;
    public const int DefaultPromptContextLimit = 10;
    public const int FilterListRetrievalCap = 100;
    public const int FilterListChunksPerProject = 8;

    private static readonly Regex BroadExperienceRegex = new(
        @"\bhave you (?:ever )?(?:used|built|worked|done)\b|\bwhat (?:kind of )?experience\b|\bwhat production experience\b|\bdo you have .{0,40}experience\b|\bhar du (?:brukt|jobbet|erfaring)\b|\bhvilken erfaring\b|\bproduksjonserfaring\b",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);

    public static bool IsFilterList(QuestionIntent intent)
    {
        return string.Equals(
            intent.Category,
            QuestionIntentDetector.FilterList,
            StringComparison.OrdinalIgnoreCase);
    }

    public static bool IsCount(QuestionIntent intent)
    {
        return string.Equals(
            intent.Category,
            QuestionIntentDetector.Count,
            StringComparison.OrdinalIgnoreCase);
    }

    public static bool IsList(QuestionIntent intent)
    {
        return string.Equals(
            intent.Category,
            QuestionIntentDetector.List,
            StringComparison.OrdinalIgnoreCase);
    }

    public static bool IsFilterListWithCount(QuestionIntent intent)
    {
        return IsFilterList(intent) && intent.RequestedCount is > 0;
    }

    public static bool IsListWithCount(QuestionIntent intent)
    {
        return IsList(intent) && intent.RequestedCount is > 0;
    }

    public static bool IsTopNList(QuestionIntent intent)
    {
        return IsFilterListWithCount(intent) || IsListWithCount(intent);
    }

    public static int RetrievalLimit(QuestionIntent intent)
    {
        return RetrievalLimit(intent, question: null);
    }

    public static int RetrievalLimit(
        QuestionIntent intent,
        string? question)
    {
        if (IsTopNList(intent))
        {
            var requested = intent.RequestedCount!.Value;

            return Math.Min(
                FilterListRetrievalCap,
                Math.Max(50, requested * FilterListChunksPerProject));
        }

        if (IsBroadExperienceQuestion(question))
        {
            return Math.Min(
                FilterListRetrievalCap,
                Math.Max(
                    50,
                    DefaultPromptContextLimit * FilterListChunksPerProject));
        }

        return DefaultRetrievalLimit;
    }

    public static IReadOnlyList<KnowledgeRetrievalItem> Select(
        QuestionIntent intent,
        IReadOnlyList<KnowledgeRetrievalItem> items)
    {
        return Select(intent, items, question: null);
    }

    public static IReadOnlyList<KnowledgeRetrievalItem> Select(
        QuestionIntent intent,
        IReadOnlyList<KnowledgeRetrievalItem> items,
        string? question)
    {
        if (IsTopNList(intent))
        {
            return UniqueProjects(items)
                .Take(intent.RequestedCount!.Value)
                .ToList();
        }

        if (IsFilterList(intent) || IsCount(intent))
        {
            return UniqueProjects(items);
        }

        if (IsBroadExperienceQuestion(question)
            && !RefersToARetrievedProject(question, items))
        {
            return UniqueProjects(items)
                .Take(DefaultPromptContextLimit)
                .ToList();
        }

        return items
            .Take(DefaultPromptContextLimit)
            .ToList();
    }

    public static bool IsBroadExperienceQuestion(string? question)
    {
        return !string.IsNullOrWhiteSpace(question)
            && BroadExperienceRegex.IsMatch(question);
    }

    private static bool RefersToARetrievedProject(
        string? question,
        IReadOnlyList<KnowledgeRetrievalItem> items)
    {
        if (string.IsNullOrWhiteSpace(question) || items.Count == 0)
        {
            return false;
        }

        var normalizedQuestion = NormalizePhrase(question);

        foreach (var group in items.GroupBy(
            item => item.Source,
            StringComparer.OrdinalIgnoreCase))
        {
            var slug = Path.GetFileNameWithoutExtension(group.Key);
            var slugPhrase = NormalizePhrase(slug.Replace('-', ' '));

            if (slugPhrase.Length >= 8
                && normalizedQuestion.Contains(slugPhrase))
            {
                return true;
            }

            var slugWithoutApi = slugPhrase.Replace(" api", "").Trim();
            if (slugWithoutApi.Length >= 8
                && normalizedQuestion.Contains(slugWithoutApi))
            {
                return true;
            }

            var title = NormalizePhrase(
                AnswerPromptFormatter.ProjectTitle(group.First()));

            if (title.Length >= 12
                && normalizedQuestion.Contains(title))
            {
                return true;
            }
        }

        return false;
    }

    private static string NormalizePhrase(string text)
    {
        return text
            .ToLowerInvariant()
            .Replace("&", "and")
            .Replace("'", "")
            .Replace(".", " ");
    }

    private static List<KnowledgeRetrievalItem> UniqueProjects(
        IReadOnlyList<KnowledgeRetrievalItem> items)
    {
        return items
            .GroupBy(
                item => item.Source,
                StringComparer.OrdinalIgnoreCase)
            .Select(SelectProjectRow)
            .OrderByDescending(row => row.ProjectScore)
            .Select(row => row.Item)
            .ToList();
    }

    private static ProjectContextRow SelectProjectRow(
        IGrouping<string, KnowledgeRetrievalItem> group)
    {
        var ordered = group
            .OrderByDescending(item => item.CombinedScore)
            .ToList();

        var overview = ordered.FirstOrDefault(
            item => string.Equals(
                item.SemanticType,
                "overview",
                StringComparison.OrdinalIgnoreCase));

        return new ProjectContextRow(
            ProjectScore: ordered[0].CombinedScore,
            Item: overview ?? ordered[0]);
    }

    private readonly record struct ProjectContextRow(
        double ProjectScore,
        KnowledgeRetrievalItem Item);
}
