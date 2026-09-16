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
        @"\bhave you (?:ever )?(?:used|built|worked|done)\b|\bwhat (?:kind of )?experiences?\b|\bwhat production experiences?\b|\bdo (?:you|i) have .{0,40}experiences?\b|\bhar du (?:brukt|jobbet|erfaring)\b|\bhvilke? erfaring(?:er)?\b|\bproduksjonserfaring\b",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);

    private static readonly Regex ProductionExperienceRegex = new(
        @"\bin production\b|\bi produksjon\b|\bproduction experience\b|\bproduksjonserfaring\b",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);

    private static readonly Regex WhatExperienceRegex = new(
        @"\bwhat (?:kind of )?experiences?\b|\bwhat production experiences?\b|\bdo (?:you|i) have .{0,40}experiences?\b|\bhvilke? erfaring(?:er)?\b",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);

    private static readonly Regex HaveYouUsedRegex = new(
        @"\bhave you (?:ever )?(?:used|built|worked|done)\b|\bhar du (?:brukt|jobbet)\b",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);

    private static readonly Regex ListedWithAndRegex = new(
        @"\b(?:and|og)\b",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);

    private static readonly Regex IntersectionCueRegex = new(
        "\\bboth\\b|\\btogether\\b|\\bin the same project\\b|\\bthe same project\\b|\\bsame project\\b|\\bb\\u00e5de\\b|\\bsamme prosjekt\\b|\\bi samme prosjekt\\b|\\bhave you (?:ever )?(?:used|built|worked).{0,80}\\bwith\\b|\\bhar du (?:brukt|jobbet).{0,80}\\bmed\\b",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);

    private static readonly Regex OrganizationLegalSuffixRegex = new(
        @"\b(?:as|ab|asa|llc|inc|ltd|gmbh)\b",
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

    public static bool IncludeMatchingOrganizationOverviews(
        QuestionIntent intent,
        string? question)
    {
        return IsList(intent)
            || IsFilterList(intent)
            || IsCount(intent)
            || IsBroadExperienceQuestion(question);
    }

    public static IReadOnlyList<string> TechnologyOverviewSlugs(
        string? question)
    {
        return TechnologyCatalog.SlugsToMerge(
            TechnologyCatalog.ResolveNamed(question));
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

        // Catalog without N: widen so a named organization (e.g. Episteme)
        // is not dropped because vector search ranked sibling docs first.
        if (IsList(intent))
        {
            return Math.Min(
                FilterListRetrievalCap,
                Math.Max(
                    50,
                    DefaultPromptContextLimit * FilterListChunksPerProject));
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
            var selected = ApplyNamedTechnologySelection(
                question,
                items,
                FilterByMentionedOrganization(question, UniqueProjects(items)));

            return selected
                .Take(intent.RequestedCount!.Value)
                .ToList();
        }

        if (IsFilterList(intent) || IsCount(intent) || IsList(intent))
        {
            return ApplyNamedTechnologySelection(
                question,
                items,
                FilterByMentionedOrganization(question, UniqueProjects(items)));
        }

        if (IsBroadExperienceQuestion(question)
            && !RefersToARetrievedProject(question, items))
        {
            var unique = FilterByMentionedOrganization(
                question,
                UniqueProjects(items));

            if (IsProductionExperienceQuestion(question))
            {
                unique = unique
                    .Where(AnswerPromptFormatter.IsProductionEnvironment)
                    .ToList();
            }

            unique = ApplyNamedTechnologySelection(question, items, unique);

            return unique
                .Take(DefaultPromptContextLimit)
                .ToList();
        }

        var detail = items
            .Take(DefaultPromptContextLimit)
            .ToList();

        if (!RefersToARetrievedProject(question, items))
        {
            detail = ApplyNamedTechnologySelection(question, items, detail);
        }

        return detail;
    }

    public static bool IsBroadExperienceQuestion(string? question)
    {
        return !string.IsNullOrWhiteSpace(question)
            && BroadExperienceRegex.IsMatch(question);
    }

    public static bool IsProductionExperienceQuestion(string? question)
    {
        return !string.IsNullOrWhiteSpace(question)
            && ProductionExperienceRegex.IsMatch(question);
    }

    public static bool IsTechIntersectionQuestion(string? question)
    {
        if (string.IsNullOrWhiteSpace(question))
        {
            return false;
        }

        if (IntersectionCueRegex.IsMatch(question))
        {
            return true;
        }

        return HaveYouUsedRegex.IsMatch(question)
            && ListedWithAndRegex.IsMatch(question);
    }

    public static bool IsTechUnionQuestion(string? question)
    {
        if (string.IsNullOrWhiteSpace(question))
        {
            return false;
        }

        return WhatExperienceRegex.IsMatch(question)
            && ListedWithAndRegex.IsMatch(question)
            && !IsTechIntersectionQuestion(question);
    }

    public static string TechListInstruction(string? question)
    {
        if (IsTechIntersectionQuestion(question))
        {
            return "For this question, the named technologies are an intersection. Only include a project if it used every named technology, according to that project's Technologies field or retrieved content. Do not use a project that has only one of them.";
        }

        if (IsTechUnionQuestion(question))
        {
            return "For this question, the named technologies are a union. Cover experience with each named technology. A project that used only one of them is relevant for that technology. Do not require every project to have used all of them. Say which of the named technologies each included project used.";
        }

        return string.Empty;
    }

    private static List<KnowledgeRetrievalItem> ApplyNamedTechnologySelection(
        string? question,
        IReadOnlyList<KnowledgeRetrievalItem> allItems,
        List<KnowledgeRetrievalItem> selected)
    {
        var named = TechnologyCatalog.ResolveNamed(question);
        if (named.Count == 0 || selected.Count == 0)
        {
            return selected;
        }

        if (RefersToARetrievedProject(question, allItems))
        {
            return selected;
        }

        var exact = IsTechIntersectionQuestion(question)
            ? selected
                .Where(item => TechnologyCatalog.ProjectHasEverySlug(item, named))
                .ToList()
            : selected
                .Where(item => TechnologyCatalog.ProjectHasAnySlug(item, named))
                .ToList();

        var missing = named
            .Where(slug => !selected.Any(item =>
                TechnologyCatalog.ProjectHasSlug(item, slug)))
            .ToList();

        if (missing.Count == 0)
        {
            return exact;
        }

        var relatedSlugs = TechnologyCatalog.SlugsToMerge(missing)
            .Where(slug => !named.Contains(slug, StringComparer.OrdinalIgnoreCase))
            .ToList();

        var related = selected
            .Where(item =>
                !TechnologyCatalog.ProjectHasAnySlug(item, named)
                && TechnologyCatalog.ProjectHasAnySlug(item, relatedSlugs))
            .ToList();

        if (exact.Count == 0)
        {
            return related
                .Take(DefaultPromptContextLimit)
                .ToList();
        }

        return exact
            .Concat(related.Take(TechnologyCatalog.RelatedFillCap))
            .ToList();
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

    private static List<KnowledgeRetrievalItem> FilterByMentionedOrganization(
        string? question,
        List<KnowledgeRetrievalItem> items)
    {
        if (string.IsNullOrWhiteSpace(question) || items.Count == 0)
        {
            return items;
        }

        var normalizedQuestion = NormalizePhrase(question);
        var mentioned = items
            .Select(item => AnswerPromptFormatter.MetadataString(item, "organization"))
            .Where(organization => !string.IsNullOrWhiteSpace(organization))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Where(organization => QuestionMentionsOrganization(
                normalizedQuestion,
                organization))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        if (mentioned.Count == 0)
        {
            return items;
        }

        return items
            .Where(item => mentioned.Contains(
                AnswerPromptFormatter.MetadataString(item, "organization")))
            .ToList();
    }

    private static bool QuestionMentionsOrganization(
        string normalizedQuestion,
        string organization)
    {
        var normalizedOrg = NormalizePhrase(organization).Trim();
        if (normalizedOrg.Length == 0)
        {
            return false;
        }

        if (normalizedQuestion.Contains(normalizedOrg))
        {
            return true;
        }

        var withoutSuffix = OrganizationLegalSuffixRegex
            .Replace(normalizedOrg, " ")
            .Trim();

        if (withoutSuffix.Length >= 4
            && normalizedQuestion.Contains(withoutSuffix))
        {
            return true;
        }

        return false;
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
