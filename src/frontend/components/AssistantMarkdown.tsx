import Markdown from "react-markdown";
import type { Components } from "react-markdown";
import remarkGfm from "remark-gfm";

/**
 * Models often insert non-breaking spaces and non-breaking hyphens
 * (Norwegian/French typography). Those block wrapping between tokens.
 */
export function restoreWrapOpportunities(text: string): string {
  return text.replace(/[\u00A0\u202F\u2007]/g, " ").replace(/\u2011/g, "-");
}

/**
 * Some models emit a GFM table as one line. Insert newlines before the
 * separator and before numbered data rows so remark-gfm can parse it.
 */
export function restoreMarkdownTableRows(text: string): string {
  return text
    .replace(/\|\s+(\|(?:[-: ]+\|){2,})/g, "|\n$1")
    .replace(/(\|(?:[-: ]+\|){2,})\s+(?=\|)/g, "$1\n")
    .replace(/\|\s+\|(?=\s*\d+\s+\|)/g, "|\n|");
}

const BARE_HTTP_URL = /https?:\/\/[^\s<>"')]+/gi;

/** Turn bare http(s) URLs into Markdown links. Skip destinations already in `[text](url)`. */
export function linkifyBareUrls(text: string): string {
  return text.replace(BARE_HTTP_URL, (url, offset: number, source: string) => {
    if (source.slice(Math.max(0, offset - 2), offset) === "](") {
      return url;
    }

    if (offset > 0 && source[offset - 1] === "<") {
      return url;
    }

    return `[${url}](${url})`;
  });
}

function prepareAssistantMarkdown(text: string): string {
  return linkifyBareUrls(
    restoreMarkdownTableRows(restoreWrapOpportunities(text)),
  );
}

const markdownComponents: Components = {
  p: ({ children }) => (
    <p className="mb-3 leading-relaxed last:mb-0">{children}</p>
  ),
  ul: ({ children }) => (
    <ul className="mb-3 list-disc space-y-1.5 pl-5 last:mb-0">{children}</ul>
  ),
  ol: ({ children }) => (
    <ol className="mb-3 list-decimal space-y-1.5 pl-5 last:mb-0">{children}</ol>
  ),
  li: ({ children }) => <li className="leading-relaxed">{children}</li>,
  strong: ({ children }) => <strong className="font-semibold">{children}</strong>,
  em: ({ children }) => <em className="italic">{children}</em>,
  del: ({ children }) => <del className="line-through">{children}</del>,
  h1: ({ children }) => (
    <h2 className="mb-2 mt-3 text-base font-semibold first:mt-0">{children}</h2>
  ),
  h2: ({ children }) => (
    <h2 className="mb-2 mt-3 text-base font-semibold first:mt-0">{children}</h2>
  ),
  h3: ({ children }) => (
    <h3 className="mb-2 mt-3 text-sm font-semibold first:mt-0">{children}</h3>
  ),
  hr: () => <hr className="my-3 border-0 border-t border-line" />,
  a: ({ href, children }) => (
    <a
      className="underline underline-offset-2 transition-colors hover:text-muted"
      href={href}
      rel="noopener noreferrer"
      target="_blank"
    >
      {children}
    </a>
  ),
  table: ({ children }) => (
    <div className="mb-3 overflow-x-auto last:mb-0">
      <table className="min-w-full border-collapse border border-line text-sm">
        {children}
      </table>
    </div>
  ),
  thead: ({ children }) => (
    <thead className="bg-surface-muted">{children}</thead>
  ),
  tbody: ({ children }) => <tbody>{children}</tbody>,
  tr: ({ children }) => <tr className="border-line">{children}</tr>,
  th: ({ children }) => (
    <th className="border border-line px-2 py-1.5 text-left font-semibold align-top">
      {children}
    </th>
  ),
  td: ({ children }) => (
    <td className="border border-line px-2 py-1.5 align-top">{children}</td>
  ),
  pre: ({ children }) => (
    <pre className="mb-3 overflow-x-auto rounded-md bg-surface-muted p-3 text-sm last:mb-0">
      {children}
    </pre>
  ),
  code: ({ className, children }) =>
    className ? (
      <code className="font-mono text-sm">{children}</code>
    ) : (
      <code className="rounded bg-surface-muted px-1 py-0.5 font-mono text-sm">
        {children}
      </code>
    ),
};

type AssistantMarkdownProps = {
  content: string;
};

export function AssistantMarkdown({ content }: AssistantMarkdownProps) {
  return (
    <div className="break-words text-ink">
      <Markdown remarkPlugins={[remarkGfm]} components={markdownComponents}>
        {prepareAssistantMarkdown(content)}
      </Markdown>
    </div>
  );
}
