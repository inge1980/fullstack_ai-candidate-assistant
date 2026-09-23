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
        var lines = answer.Replace("\r\n", "\n", StringComparison.Ordinal)
            .Replace('\r', '\n')
            .Split('\n');

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
}
