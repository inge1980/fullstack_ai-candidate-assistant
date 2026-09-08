import { useTranslation } from "react-i18next";
import { AssistantMarkdown } from "./AssistantMarkdown";

export type ChatMessage = {
  id: string;
  role: "user" | "assistant";
  content: string;
};

type MessageListProps = {
  messages: ChatMessage[];
};

export function MessageList({ messages }: MessageListProps) {
  const { t } = useTranslation();

  if (messages.length === 0) {
    return null;
  }

  return (
    <ul className="flex flex-col gap-3" aria-live="polite">
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
            <AssistantMarkdown content={message.content} />
          ) : (
            <p className="whitespace-pre-wrap">{message.content}</p>
          )}
        </li>
      ))}
    </ul>
  );
}

