using System.Text.Json;
using Application.Knowledge;

namespace Application.Questions;

public static class AnswerPromptFormatter
{
    public static string FormatContext(
        IReadOnlyList<KnowledgeRetrievalItem> items)
    {
        return string.Join(
            "\n\n",
            items.Select(
                (result, index) =>
                    $"[{index + 1}] {result.Source}\n" +
                    $"Project: {ProjectTitle(result)}\n" +
                    $"Organization: {MetadataString(result, "organization")}\n" +
                    $"Environment: {MetadataString(result, "environment")}\n" +
                    $"Technologies: {ProjectTechnologies(result)}\n" +
                    LinksLine(result) +
                    $"Heading: {result.Heading}\n" +
                    $"Semantic Type: {result.SemanticType}\n" +
                    $"Content: {result.Content}"));
    }

    public static string Fill(
        string template,
        string question,
        string context,
        string locale)
    {
        return template
            .Replace("{{question}}", question)
            .Replace("{{context}}", context)
            .Replace(
                "{{answer_language_instruction}}",
                QuestionLocale.AnswerLanguageInstruction(locale))
            .Replace(
                "{{tech_list_instruction}}",
                PromptContextSelector.TechListInstruction(question));
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
        if (!result.Metadata.TryGetValue("links", out var value)
            || value is null)
        {
            return string.Empty;
        }

        if (value is JsonElement json
            && json.ValueKind == JsonValueKind.Object)
        {
            return string.Join(
                ", ",
                json.EnumerateObject()
                    .Select(property => FormatLinkEntry(
                        property.Name,
                        property.Value.ValueKind == JsonValueKind.String
                            ? property.Value.GetString()
                            : property.Value.ToString()))
                    .Where(static text => !string.IsNullOrWhiteSpace(text)));
        }

        if (value is IDictionary<string, object?> dictionary)
        {
            return string.Join(
                ", ",
                dictionary
                    .Select(pair => FormatLinkEntry(
                        pair.Key,
                        pair.Value?.ToString()))
                    .Where(static text => !string.IsNullOrWhiteSpace(text)));
        }

        return value.ToString() ?? string.Empty;
    }

    private static string LinksLine(KnowledgeRetrievalItem result)
    {
        var links = ProjectLinks(result);
        return string.IsNullOrWhiteSpace(links)
            ? string.Empty
            : $"Links: {links}\n";
    }

    private static string? FormatLinkEntry(string name, string? url)
    {
        if (string.IsNullOrWhiteSpace(name)
            || string.IsNullOrWhiteSpace(url))
        {
            return null;
        }

        return $"{name}: {url.Trim()}";
    }
}
