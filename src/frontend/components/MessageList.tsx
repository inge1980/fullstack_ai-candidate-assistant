import { useTranslation } from "react-i18next";
import { AssistantMarkdown } from "./AssistantMarkdown";

export type ChatMessage = {
  id: string;
  role: "user" | "assistant";
  content: string;
  prompt?: string;
};

type MessageListProps = {
  messages: ChatMessage[];
  isLoading?: boolean;
};

function GeneratingIndicator() {
  const { t } = useTranslation();

  return (
    <li
      className="mr-8 rounded-md border border-zinc-200 bg-white px-3 py-2 text-zinc-900"
      aria-live="polite"
    >
      <p className="mb-1 text-xs font-medium uppercase tracking-wide opacity-70">
        M.I.N.D
      </p>
      <div className="flex items-center gap-3" role="status">
        <svg
          className="size-5 shrink-0 animate-spin text-zinc-500"
          viewBox="0 0 24 24"
          fill="none"
          aria-hidden="true"
        >
          <circle
            className="opacity-25"
            cx="12"
            cy="12"
            r="10"
            stroke="currentColor"
            strokeWidth="3"
          />
          <path
            className="opacity-90"
            fill="currentColor"
            d="M12 2a10 10 0 0 1 10 10h-3a7 7 0 0 0-7-7V2z"
          />
        </svg>
        <p className="text-sm text-zinc-700">{t("status.loading")}</p>
      </div>
    </li>
  );
}

export function MessageList({ messages, isLoading = false }: MessageListProps) {
  const { t } = useTranslation();

  if (messages.length === 0 && !isLoading) {
    return null;
  }

  return (
    <ul className="flex flex-col gap-3">
      {messages.map((message) => (
        <li
          key={message.id}
          className={
            message.role === "user"
              ? "ml-8 rounded-md bg-zinc-900 px-3 py-2 text-white"
              : "mr-8 rounded-md border border-zinc-200 bg-white px-3 py-2 text-zinc-900"
          }
        >
          <p className="mb-1 text-xs font-medium uppercase tracking-wide opacity-70">
            {message.role === "user" ? t("chat.you") : "M.I.N.D"}
          </p>
          {message.role === "assistant" ? (
            <>
              <AssistantMarkdown content={message.content} />
              {message.prompt ? (
                <details className="mt-3 border-t border-zinc-200 pt-2">
                  <summary className="cursor-pointer text-xs font-medium text-zinc-600">
                    {t("chat.debugPrompt")}
                  </summary>
                  <pre className="mt-2 max-h-96 overflow-auto whitespace-pre-wrap break-words text-xs text-zinc-700">
                    {message.prompt}
                  </pre>
                </details>
              ) : null}
            </>
          ) : (
            <p className="whitespace-pre-wrap">{message.content}</p>
          )}
        </li>
      ))}
      {isLoading ? <GeneratingIndicator /> : null}
    </ul>
  );
}

