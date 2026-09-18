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

        return string.Join(
            "\n\n",
            items.Select(
                (result, index) =>
                    $"[{index + 1}] {result.Source}\n" +
                    $"Project: {ProjectTitle(result)}\n" +
                    $"Organization: {MetadataString(result, "organization")}\n" +
                    $"Period: {FormatProjectPeriod(result)}\n" +
                    $"Environment: {MetadataString(result, "environment")}\n" +
                    $"Technologies: {ProjectTechnologies(result)}\n" +
                    TechnologyCatalog.FormatMatchLine(result, named) +
                    LinksLine(result) +
                    $"Heading: {result.Heading}\n" +
                    $"Semantic Type: {result.SemanticType}\n" +
                    $"Content: {result.Content}"));
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
        if (!result.Metadata.TryGetValue("period", out var value)
            || value is not JsonElement json
            || json.ValueKind != JsonValueKind.Object)
        {
            return string.Empty;
        }

        var fromText = json.TryGetProperty("from", out var fromField)
            ? fromField.GetString()?.Trim()
            : null;
        var toText = json.TryGetProperty("to", out var toField)
            ? toField.GetString()?.Trim()
            : null;
        if (string.IsNullOrWhiteSpace(fromText)
            || string.IsNullOrWhiteSpace(toText))
        {
            return string.Empty;
        }

        var span = $"{fromText} to {toText}";
        if (!DateTime.TryParseExact(
                fromText,
                "yyyy-MM",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var fromDate))
        {
            return span;
        }

        DateTime toDate;
        if (toText.Equals("Present", StringComparison.OrdinalIgnoreCase))
        {
            var now = DateTime.UtcNow;
            toDate = new DateTime(now.Year, now.Month, 1);
        }
        else if (!DateTime.TryParseExact(
            toText,
            "yyyy-MM",
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out toDate))
        {
            return span;
        }

        var months =
            (toDate.Year - fromDate.Year) * 12
            + (toDate.Month - fromDate.Month);
        if (months < 12)
        {
            return span;
        }

        var years = months / 12;
        return $"{span} ({years} {(years == 1 ? "year" : "years")})";
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
