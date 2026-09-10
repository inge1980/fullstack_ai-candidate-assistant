using Application.Knowledge;

namespace Application.Questions;

public static class PromptContextSelector
{
    public const int DefaultRetrievalLimit = 25;
    public const int DefaultPromptContextLimit = 10;
    public const int FilterListRetrievalCap = 100;
    public const int FilterListChunksPerProject = 8;

    public static bool IsFilterListWithCount(QuestionIntent intent)
    {
        return string.Equals(
                intent.Category,
                QuestionIntentDetector.FilterList,
                StringComparison.OrdinalIgnoreCase)
            && intent.RequestedCount is > 0;
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
        if (!IsFilterListWithCount(intent))
        {
            return items
                .Take(DefaultPromptContextLimit)
                .ToList();
        }

        var take = intent.RequestedCount!.Value;

        return items
            .GroupBy(
                item => item.Source,
                StringComparer.OrdinalIgnoreCase)
            .Select(SelectProjectRow)
            .OrderByDescending(row => row.ProjectScore)
            .Take(take)
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
