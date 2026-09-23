using System.Globalization;
using System.Text.Json;
using Application.Knowledge;

namespace Application.Questions;

public static class AnswerPromptFormatter
{
    public static string FormatContext(
        IReadOnlyList<KnowledgeRetrievalItem> items,
        string? question = null)
    {
        if (items.Count == 0)
        {
            return "(no retrieved evidence - do not invent projects, technologies, or URLs)";
        }

        var named = TechnologyCatalog.ResolveNamed(question);
        var chunks = string.Join(
            "\n\n",
            items.Select(
                (result, index) =>
                    $"[{index + 1}] {result.Source}\n" +
                    $"Project: {ProjectTitle(result)}\n" +
                    $"Organization: {MetadataString(result, "organization")}\n" +
                    $"Period: {FormatProjectPeriod(result)}\n" +
                    $"Status: {MetadataString(result, "status")}\n" +
                    $"Environment: {MetadataString(result, "environment")}\n" +
                    $"Technologies: {ProjectTechnologies(result)}\n" +
                    TechnologyCatalog.FormatMatchLine(result, named) +
                    LinksLine(result) +
                    $"Heading: {result.Heading}\n" +
                    $"Semantic Type: {result.SemanticType}\n" +
                    $"Content: {result.Content}"));

        var header = FormatContextHeader(items, question);
        return string.IsNullOrWhiteSpace(header)
            ? chunks
            : header + "\n\n" + chunks;
    }

    public static bool IsCompanyOrganization(KnowledgeRetrievalItem result)
    {
        var organization = MetadataString(result, "organization").Trim();
        return organization.Length > 0
            && !organization.Equals(
                "Personal Project",
                StringComparison.OrdinalIgnoreCase)
            && !organization.Equals(
                "School Project",
                StringComparison.OrdinalIgnoreCase);
    }

    private static string FormatContextHeader(
        IReadOnlyList<KnowledgeRetrievalItem> items,
        string? question)
    {
        var spans = FormatOrganizationSpans(items, question);
        if (!string.IsNullOrWhiteSpace(spans))
        {
            return spans;
        }

        return FormatOverlappingPeriod(items);
    }

    public static string Fill(
        string template,
        string question,
        string context,
        string locale,
        IReadOnlyList<KnowledgeRetrievalItem>? promptItems = null)
    {
        return template
            .Replace("{{question}}", question)
            .Replace("{{context}}", context)
            .Replace(
                "{{answer_language_instruction}}",
                QuestionLocale.AnswerLanguageInstruction(locale))
            .Replace(
                "{{tech_list_instruction}}",
                PromptContextSelector.TechListInstruction(question))
            .Replace(
                "{{tech_match_instruction}}",
                TechnologyCatalog.MatchInstruction(question, promptItems));
    }

    public static string ProjectId(string source)
    {
        return Path.GetFileNameWithoutExtension(source);
    }

    public static string ProjectTitle(KnowledgeRetrievalItem result)
    {
        var title = MetadataString(result, "title");
        return string.IsNullOrWhiteSpace(title)
            ? ProjectId(result.Source)
            : title;
    }

    public static bool IsProductionEnvironment(KnowledgeRetrievalItem result)
    {
        return string.Equals(
            MetadataString(result, "environment").Trim(),
            "production",
            StringComparison.OrdinalIgnoreCase);
    }

    private static string FormatProjectPeriod(KnowledgeRetrievalItem result)
    {
        return TryReadPeriod(result, out var range)
            ? FormatYearSpan(range.FromText, range.ToText, range.MonthSpan)
            : string.Empty;
    }

    private static string FormatOverlappingPeriod(
        IReadOnlyList<KnowledgeRetrievalItem> items)
    {
        var organizations = items
            .Select(item => MetadataString(item, "organization").Trim())
            .Where(static organization => organization.Length > 0)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
        if (organizations.Count != 1)
        {
            return string.Empty;
        }

        var span = TryMergeSpan(items);
        return span is null
            ? string.Empty
            : "OverlappingPeriod: " + span.Value.Text;
    }

    private static string FormatOrganizationSpans(
        IReadOnlyList<KnowledgeRetrievalItem> items,
        string? question)
    {
        if (!PromptContextSelector.IsProfessionalCatalogQuestion(question))
        {
            return string.Empty;
        }

        var groups = items
            .Where(IsCompanyOrganization)
            .GroupBy(
                item => MetadataString(item, "organization").Trim(),
                StringComparer.OrdinalIgnoreCase)
            .Select(group => (Organization: group.Key, Span: TryMergeSpan(group.ToList())))
            .ToList();

        var spanLines = groups
            .Where(static line => line.Span is not null)
            .OrderBy(static line => line.Span!.Value.FromMonths)
            .ThenBy(static line => line.Organization, StringComparer.OrdinalIgnoreCase)
            .Select(static line => "- " + line.Organization + ": " + line.Span!.Value.Text)
            .ToList();

        var shorterLines = groups
            .Where(static line => line.Span is null)
            .OrderBy(static line => line.Organization, StringComparer.OrdinalIgnoreCase)
            .Select(static line => "- " + line.Organization)
            .ToList();

        var blocks = new List<string>();
        if (spanLines.Count > 0)
        {
            blocks.Add("OrganizationSpans:\n" + string.Join("\n", spanLines));
        }

        if (shorterLines.Count > 0)
        {
            blocks.Add("ShorterOrganizations:\n" + string.Join("\n", shorterLines));
        }

        return string.Join("\n", blocks);
    }

    private static MergedSpan? TryMergeSpan(
        IReadOnlyList<KnowledgeRetrievalItem> items)
    {
        var ranges = items
            .Select(item => TryReadPeriod(item, out var range) ? range : (PeriodRange?)null)
            .Where(static range => range is not null)
            .Select(static range => range!.Value)
            .OrderBy(static range => range.FromMonths)
            .ToList();
        if (ranges.Count == 0)
        {
            return null;
        }

        var merged = MergeOverlapping(ranges);
        if (merged.Count != 1 || merged[0].MonthSpan < 24)
        {
            return null;
        }

        var span = merged[0];
        return new MergedSpan(
            span.FromMonths,
            FormatYearSpan(span.FromText, span.ToText, span.MonthSpan));
    }

    private static List<PeriodRange> MergeOverlapping(List<PeriodRange> ranges)
    {
        var merged = new List<PeriodRange>();
        foreach (var range in ranges)
        {
            if (merged.Count == 0
                || range.FromMonths > merged[^1].ToMonths + 1)
            {
                merged.Add(range);
                continue;
            }

            var last = merged[^1];
            if (range.ToMonths > last.ToMonths)
            {
                merged[^1] = last with
                {
                    ToMonths = range.ToMonths,
                    ToText = range.ToText
                };
            }
        }

        return merged;
    }

    private static bool TryReadPeriod(
        KnowledgeRetrievalItem result,
        out PeriodRange range)
    {
        range = default;
        if (!result.Metadata.TryGetValue("period", out var value)
            || value is not JsonElement json
            || json.ValueKind != JsonValueKind.Object)
        {
            return false;
        }

        var fromText = json.TryGetProperty("from", out var fromField)
            ? fromField.GetString()?.Trim()
            : null;
        var toText = json.TryGetProperty("to", out var toField)
            ? toField.GetString()?.Trim()
            : null;
        if (string.IsNullOrWhiteSpace(fromText)
            || string.IsNullOrWhiteSpace(toText)
            || !TryParseYearMonth(fromText, out var fromDate)
            || !TryParsePeriodEnd(toText, out var toDate))
        {
            return false;
        }

        range = new PeriodRange(
            FromMonths: fromDate.Year * 12 + fromDate.Month,
            ToMonths: toDate.Year * 12 + toDate.Month,
            FromText: fromText,
            ToText: toText);
        return range.ToMonths >= range.FromMonths;
    }

    private static bool TryParseYearMonth(string text, out DateTime date)
    {
        return DateTime.TryParseExact(
            text,
            "yyyy-MM",
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out date);
    }

    private static bool TryParsePeriodEnd(string toText, out DateTime date)
    {
        if (toText.Equals("Present", StringComparison.OrdinalIgnoreCase))
        {
            var now = DateTime.UtcNow;
            date = new DateTime(now.Year, now.Month, 1);
            return true;
        }

        return TryParseYearMonth(toText, out date);
    }

    private static string FormatYearSpan(
        string fromText,
        string toText,
        int months)
    {
        var span = $"{fromText} to {toText}";
        if (months < 12)
        {
            return span;
        }

        var years = months / 12;
        return $"{span} ({years} {(years == 1 ? "year" : "years")})";
    }

    private readonly record struct MergedSpan(int FromMonths, string Text);

    private readonly record struct PeriodRange(
        int FromMonths,
        int ToMonths,
        string FromText,
        string ToText)
    {
        public int MonthSpan => ToMonths - FromMonths;
    }

    public static string MetadataString(
        KnowledgeRetrievalItem result,
        string key)
    {
        if (!result.Metadata.TryGetValue(key, out var value)
            || value is null)
        {
            return string.Empty;
        }

        if (value is JsonElement json)
        {
            if (json.ValueKind == JsonValueKind.String)
            {
                return json.GetString() ?? string.Empty;
            }

            return json.ToString();
        }

        return value.ToString() ?? string.Empty;
    }

    public static string ProjectTechnologies(KnowledgeRetrievalItem result)
    {
        if (!result.Metadata.TryGetValue(
                "technologies",
                out var value)
            || value is null)
        {
            return string.Empty;
        }

        if (value is JsonElement json
            && json.ValueKind == JsonValueKind.Array)
        {
            return string.Join(
                ", ",
                json.EnumerateArray()
                    .Select(item => item.GetString())
                    .Where(static text => !string.IsNullOrWhiteSpace(text)));
        }

        return value.ToString() ?? string.Empty;
    }

    public static string ProjectLinks(KnowledgeRetrievalItem result)
    {
        var links = KnownHttpLinks(result);
        if (links.Count == 0)
        {
            return string.Empty;
        }

        var title = ProjectTitle(result);
        var titleAssigned = false;
        var lines = new List<string>();

        foreach (var kind in LinkKindOrder)
        {
            if (!links.TryGetValue(kind, out var url))
            {
                continue;
            }

            var useProjectTitle = !titleAssigned;
            titleAssigned = true;
            lines.Add(FormatKnownLinkLine(kind, title, url, useProjectTitle));
        }

        return string.Join("\n", lines);
    }

    private static readonly string[] LinkKindOrder =
    [
        "github",
        "live",
        "portfolio"
    ];

    private static string LinksLine(KnowledgeRetrievalItem result)
    {
        var links = ProjectLinks(result);
        return string.IsNullOrWhiteSpace(links)
            ? string.Empty
            : $"Links:\n{links}\n";
    }

    private static Dictionary<string, string> KnownHttpLinks(
        KnowledgeRetrievalItem result)
    {
        var links = new Dictionary<string, string>(StringComparer.Ordinal);

        foreach (var (name, url) in EnumerateLinkPairs(result))
        {
            var kind = NormalizeLinkKind(name);
            if (kind is null
                || url is null
                || !IsHttpUrl(url))
            {
                continue;
            }

            links[kind] = url.Trim();
        }

        return links;
    }

    private static IEnumerable<(string Name, string? Url)> EnumerateLinkPairs(
        KnowledgeRetrievalItem result)
    {
        if (!result.Metadata.TryGetValue("links", out var value)
            || value is null)
        {
            yield break;
        }

        if (value is JsonElement json
            && json.ValueKind == JsonValueKind.Object)
        {
            foreach (var property in json.EnumerateObject())
            {
                var url = property.Value.ValueKind == JsonValueKind.String
                    ? property.Value.GetString()
                    : property.Value.ToString();
                yield return (property.Name, url);
            }

            yield break;
        }

        if (value is IDictionary<string, object?> dictionary)
        {
            foreach (var pair in dictionary)
            {
                yield return (pair.Key, pair.Value?.ToString());
            }
        }
    }

    private static string? NormalizeLinkKind(string name)
    {
        return name.Trim().ToLowerInvariant() switch
        {
            "github" or "code" => "github",
            "live" or "demo" => "live",
            "portfolio" => "portfolio",
            _ => null
        };
    }

    private static bool IsHttpUrl(string? url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return false;
        }

        var trimmed = url.Trim();
        if (trimmed.Equals("Not available", StringComparison.OrdinalIgnoreCase)
            || trimmed.Equals("n/a", StringComparison.OrdinalIgnoreCase)
            || trimmed.Equals("none", StringComparison.OrdinalIgnoreCase)
            || trimmed.Equals("null", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        return Uri.TryCreate(trimmed, UriKind.Absolute, out var uri)
            && (uri.Scheme == Uri.UriSchemeHttp
                || uri.Scheme == Uri.UriSchemeHttps);
    }

    private static string FormatKnownLinkLine(
        string kind,
        string projectTitle,
        string url,
        bool useProjectTitle)
    {
        var (label, typeText) = kind switch
        {
            "github" => ("GitHub (code)", "GitHub"),
            "live" => ("Live (demo)", "demo"),
            "portfolio" => ("Portfolio (article)", "portfolio"),
            _ => (kind, kind)
        };

        var text = useProjectTitle ? projectTitle : typeText;
        return $"- {label}: [{text}]({url})";
    }
}
