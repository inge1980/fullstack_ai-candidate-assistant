using Application.Knowledge;

namespace Application.Questions;

public static class PromptContextSelector
{
    public const int DefaultRetrievalLimit = 25;
    public const int DefaultPromptContextLimit = 10;
    public const int FilterListRetrievalCap = 100;
    public const int FilterListChunksPerProject = 8;

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

    public static bool IsFilterListWithCount(QuestionIntent intent)
    {
        return IsFilterList(intent) && intent.RequestedCount is > 0;
    }

    public static int RetrievalLimit(QuestionIntent intent)
    {
        if (!IsFilterListWithCount(intent))
        {
            return DefaultRetrievalLimit;
        }

        var requested = intent.RequestedCount!.Value;

        return Math.Min(
            FilterListRetrievalCap,
            Math.Max(50, requested * FilterListChunksPerProject));
    }

    public static IReadOnlyList<KnowledgeRetrievalItem> Select(
        QuestionIntent intent,
        IReadOnlyList<KnowledgeRetrievalItem> items)
    {
        if (IsFilterListWithCount(intent))
        {
            return UniqueProjects(items)
                .Take(intent.RequestedCount!.Value)
                .ToList();
        }

        if (IsFilterList(intent) || IsCount(intent))
        {
            return UniqueProjects(items);
        }

        return items
            .Take(DefaultPromptContextLimit)
            .ToList();
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
