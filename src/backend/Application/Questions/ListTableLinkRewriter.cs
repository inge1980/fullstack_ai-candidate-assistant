using System.Text;
using System.Text.RegularExpressions;
using Application.Knowledge;

namespace Application.Questions;

public static class ListTableLinkRewriter
{
    private static readonly Regex MarkdownLink = new(
        @"\[([^\]]*)\]\([^)]*\)",
        RegexOptions.CultureInvariant | RegexOptions.Compiled);

    private static readonly Regex SeparatorCell = new(
        @"^:?-{3,}:?$",
        RegexOptions.CultureInvariant | RegexOptions.Compiled);

    private static readonly Regex SentenceBoundary = new(
        @"(?<=[.!?])\s+",
        RegexOptions.CultureInvariant | RegexOptions.Compiled);

    private static readonly Regex LeadingParenthetical = new(
        @"^\([^)]*\)\s*",
        RegexOptions.CultureInvariant | RegexOptions.Compiled);

    public static string Apply(
        string? answer,
        IReadOnlyList<KnowledgeRetrievalItem> items,
        string locale)
    {
        if (string.IsNullOrEmpty(answer) || items.Count == 0)
        {
            return answer ?? string.Empty;
        }

        locale = QuestionLocale.Normalize(locale);
        var lines = Lines(answer);

        for (var i = 0; i < lines.Length; i++)
        {
            if (!TryReadProjectTable(lines[i], out var headers, out var projectIndex))
            {
                continue;
            }

            if (i + 1 >= lines.Length || !IsSeparator(lines[i + 1]))
            {
                continue;
            }

            var articleIndex = IndexOfArticle(headers);
            var omitPortfolio = articleIndex >= 0;
            i += 2;
            while (i < lines.Length && IsTableRow(lines[i]))
            {
                if (!IsSeparator(lines[i]))
                {
                    lines[i] = RewriteRow(
                        lines[i],
                        items,
                        locale,
                        projectIndex,
                        omitPortfolio,
                        articleIndex);
                }

                i++;
            }

            i--;
        }

        return string.Join("\n", lines);
    }

    public static bool ContainsProjectTable(string? answer)
    {
        if (string.IsNullOrEmpty(answer))
        {
            return false;
        }

        var lines = Lines(answer);
        for (var i = 0; i < lines.Length; i++)
        {
            if (!TryReadProjectTable(lines[i], out _, out _))
            {
                continue;
            }

            if (i + 1 < lines.Length && IsSeparator(lines[i + 1]))
            {
                return true;
            }
        }

        return false;
    }

    public static string BuildCatalogTable(
        string? answer,
        IReadOnlyList<KnowledgeRetrievalItem> items,
        string locale,
        string? question)
    {
        var projects = PromptContextSelector.MatchingProjects(question, items);
        if (projects.Count == 0)
        {
            return answer ?? string.Empty;
        }

        locale = QuestionLocale.Normalize(locale);
        var useArticle = projects.Count(item =>
            AnswerPromptFormatter.PortfolioArticleLink(item, locale).Length > 0) > 2;
        var articleHeader = locale == QuestionLocale.Nb ? "Artikkel" : "Article";
        var table = new StringBuilder();
        table.Append("| Project | Summary |");
        if (useArticle)
        {
            table.Append(' ').Append(articleHeader).Append(" |");
        }

        table.Append('\n');
        table.Append("| --- | --- |");
        if (useArticle)
        {
            table.Append(" --- |");
        }

        table.Append('\n');
        foreach (var project in projects)
        {
            var title = AnswerPromptFormatter.ProjectTitle(project);
            var summary = SummaryFromAnswer(answer, title);
            if (summary.Length == 0)
            {
                summary = TakeSentences(project.Content, 2);
            }

            table.Append("| ")
                .Append(EscapeCell(
                    AnswerPromptFormatter.FormatLinkHeading(project, useArticle, locale)))
                .Append(" | ")
                .Append(EscapeCell(summary))
                .Append(" |");
            if (useArticle)
            {
                table.Append(' ')
                    .Append(AnswerPromptFormatter.PortfolioArticleLink(project, locale))
                    .Append(" |");
            }

            table.Append('\n');
        }

        var opening = OpeningSentences(answer);
        return opening.Length == 0
            ? table.ToString().TrimEnd()
            : opening + "\n\n" + table.ToString().TrimEnd();
    }

    private static string RewriteRow(
        string line,
        IReadOnlyList<KnowledgeRetrievalItem> items,
        string locale,
        int projectIndex,
        bool omitPortfolio,
        int articleIndex)
    {
        var cells = SplitCells(line);
        if (projectIndex < 0 || projectIndex >= cells.Count)
        {
            return line;
        }

        var match = MatchProject(cells[projectIndex], items);
        if (match is null)
        {
            return line;
        }

        cells[projectIndex] = EscapeCell(
            AnswerPromptFormatter.FormatLinkHeading(match, omitPortfolio, locale));

        if (omitPortfolio
            && articleIndex >= 0
            && articleIndex < cells.Count
            && string.IsNullOrWhiteSpace(cells[articleIndex]))
        {
            var article = AnswerPromptFormatter.PortfolioArticleLink(match, locale);
            if (!string.IsNullOrEmpty(article))
            {
                cells[articleIndex] = article;
            }
        }

        return "| " + string.Join(" | ", cells) + " |";
    }

    private static KnowledgeRetrievalItem? MatchProject(
        string cell,
        IReadOnlyList<KnowledgeRetrievalItem> items)
    {
        var visible = Collapse(MarkdownLink.Replace(cell, "$1"));
        if (visible.Length == 0)
        {
            return null;
        }

        KnowledgeRetrievalItem? best = null;
        var bestLength = 0;
        foreach (var item in items)
        {
            var title = Collapse(AnswerPromptFormatter.ProjectTitle(item));
            if (title.Length <= bestLength)
            {
                continue;
            }

            if (visible.Contains(title, StringComparison.OrdinalIgnoreCase))
            {
                best = item;
                bestLength = title.Length;
            }
        }

        return best;
    }

    private static bool TryReadProjectTable(
        string line,
        out List<string> headers,
        out int projectIndex)
    {
        headers = [];
        projectIndex = -1;
        if (!IsTableRow(line) || IsSeparator(line))
        {
            return false;
        }

        headers = SplitCells(line);
        for (var i = 0; i < headers.Count; i++)
        {
            if (IsProjectHeader(headers[i]))
            {
                projectIndex = i;
                return true;
            }
        }

        return false;
    }

    private static int IndexOfArticle(IReadOnlyList<string> headers)
    {
        for (var i = 0; i < headers.Count; i++)
        {
            if (IsArticleHeader(headers[i]))
            {
                return i;
            }
        }

        return -1;
    }

    private static bool IsProjectHeader(string cell)
    {
        var label = HeaderLabel(cell);
        return label is "project" or "prosjekt" or "title" or "tittel";
    }

    private static bool IsArticleHeader(string cell)
    {
        var label = HeaderLabel(cell);
        return label is "article" or "artikkel" or "portfolio" or "portefølje";
    }

    private static string HeaderLabel(string cell)
    {
        var text = MarkdownLink.Replace(cell, "$1");
        return Collapse(text.Replace("*", "", StringComparison.Ordinal)
            .Replace("_", "", StringComparison.Ordinal))
            .ToLowerInvariant();
    }

    private static bool IsSeparator(string line)
    {
        if (!IsTableRow(line))
        {
            return false;
        }

        var cells = SplitCells(line);
        return cells.Count > 0 && cells.All(cell => SeparatorCell.IsMatch(cell.Trim()));
    }

    private static bool IsTableRow(string line)
    {
        return line.TrimStart().StartsWith('|');
    }

    private static List<string> SplitCells(string line)
    {
        var parts = line.Split('|');
        var start = 0;
        var end = parts.Length;
        if (end > 0 && parts[0].Trim().Length == 0)
        {
            start = 1;
        }

        if (end > start && parts[end - 1].Trim().Length == 0)
        {
            end--;
        }

        var cells = new List<string>();
        for (var i = start; i < end; i++)
        {
            cells.Add(parts[i].Trim());
        }

        return cells;
    }

    private static string EscapeCell(string value)
    {
        return value.Replace("|", "\\|", StringComparison.Ordinal);
    }

    private static string Collapse(string text)
    {
        return string.Join(
            ' ',
            text.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries));
    }

    private static string[] Lines(string answer)
    {
        return answer.Replace("\r\n", "\n", StringComparison.Ordinal)
            .Replace('\r', '\n')
            .Split('\n');
    }

    private static string OpeningSentences(string? answer)
    {
        if (string.IsNullOrWhiteSpace(answer))
        {
            return string.Empty;
        }

        var prose = new StringBuilder();
        foreach (var line in Lines(answer))
        {
            var trimmed = line.Trim();
            if (trimmed.Length == 0)
            {
                if (prose.Length > 0)
                {
                    break;
                }

                continue;
            }

            if (IsStructuralLine(trimmed))
            {
                break;
            }

            if (prose.Length > 0)
            {
                prose.Append(' ');
            }

            prose.Append(trimmed);
        }

        return TakeSentences(prose.ToString(), 2);
    }

    private static string SummaryFromAnswer(string? answer, string title)
    {
        if (string.IsNullOrWhiteSpace(answer) || title.Length == 0)
        {
            return string.Empty;
        }

        var collapsedTitle = Collapse(title);
        string? fallback = null;
        foreach (var line in Lines(answer))
        {
            var visible = VisibleText(line);
            var rest = DescriptionAfterTitle(visible, collapsedTitle);
            if (rest.Length < 12)
            {
                continue;
            }

            var summary = TakeSentences(rest, 2);
            if (IsStructuralLine(line.Trim()))
            {
                return summary;
            }

            fallback ??= summary;
        }

        return fallback ?? string.Empty;
    }

    private static string DescriptionAfterTitle(string visible, string title)
    {
        var index = visible.IndexOf(title, StringComparison.OrdinalIgnoreCase);
        if (index < 0)
        {
            return string.Empty;
        }

        var rest = visible[(index + title.Length)..].Trim();
        rest = LeadingParenthetical.Replace(rest, string.Empty);
        return rest.TrimStart(':', '-', '.', ' ').Trim();
    }

    private static string VisibleText(string line)
    {
        var text = MarkdownLink.Replace(line, "$1");
        return Collapse(text
            .Replace("*", string.Empty, StringComparison.Ordinal)
            .Replace("_", string.Empty, StringComparison.Ordinal)
            .Replace("`", string.Empty, StringComparison.Ordinal));
    }

    private static bool IsStructuralLine(string trimmed)
    {
        return trimmed.StartsWith('|')
            || trimmed.StartsWith("- ", StringComparison.Ordinal)
            || trimmed.StartsWith("* ", StringComparison.Ordinal)
            || trimmed.StartsWith('#')
            || (trimmed.Length > 2
                && char.IsDigit(trimmed[0])
                && trimmed.Contains(". ", StringComparison.Ordinal));
    }

    private static string TakeSentences(string? text, int count)
    {
        var collapsed = Collapse(text ?? string.Empty);
        if (collapsed.Length == 0 || count <= 0)
        {
            return string.Empty;
        }

        var parts = SentenceBoundary.Split(collapsed);
        if (parts.Length == 1)
        {
            return TrimWords(parts[0], 420);
        }

        return string.Join(' ', parts.Take(count));
    }

    private static string TrimWords(string text, int max)
    {
        if (text.Length <= max)
        {
            return text;
        }

        var cut = text.LastIndexOf(' ', Math.Min(max, text.Length - 1));
        if (cut < 80)
        {
            cut = max;
        }

        return text[..cut].TrimEnd() + ".";
    }
}
