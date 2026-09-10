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
                    $"Technologies: {ProjectTechnologies(result)}\n" +
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
                QuestionLocale.AnswerLanguageInstruction(locale));
    }

    public static string ProjectId(string source)
    {
        return Path.GetFileNameWithoutExtension(source);
    }

    public static string ProjectTitle(KnowledgeRetrievalItem result)
    {
        if (result.Metadata.TryGetValue(
                "title",
                out var title)
            && title is string titleString
            && !string.IsNullOrWhiteSpace(titleString))
        {
            return titleString;
        }

        return ProjectId(result.Source);
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

        if (value is System.Text.Json.JsonElement json
            && json.ValueKind == System.Text.Json.JsonValueKind.Array)
        {
            return string.Join(
                ", ",
                json.EnumerateArray()
                    .Select(item => item.GetString())
                    .Where(static text => !string.IsNullOrWhiteSpace(text)));
        }

        return value.ToString() ?? string.Empty;
    }
}
