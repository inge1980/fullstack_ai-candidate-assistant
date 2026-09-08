import { useEffect, useId, useRef } from "react";
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
  isEditingUser?: boolean;
  editValue?: string;
  onEditValueChange?: (value: string) => void;
  onStartEdit?: () => void;
  onCancelEdit?: () => void;
  onResend?: () => void;
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

function UserQuestion({
  content,
  isEditing,
  canEdit,
  editValue,
  onEditValueChange,
  onStartEdit,
  onCancelEdit,
  onResend,
}: {
  content: string;
  isEditing: boolean;
  canEdit: boolean;
  editValue: string;
  onEditValueChange: (value: string) => void;
  onStartEdit: () => void;
  onCancelEdit: () => void;
  onResend: () => void;
}) {
  const { t } = useTranslation();
  const fieldId = useId();
  const textareaRef = useRef<HTMLTextAreaElement>(null);

  useEffect(() => {
    if (!isEditing) {
      return;
    }

    const field = textareaRef.current;
    if (!field) {
      return;
    }

    field.focus();
    const length = field.value.length;
    field.setSelectionRange(length, length);
  }, [isEditing]);

  return (
    <>
      <div className="mb-1 flex items-center justify-between gap-2">
        <p className="text-xs font-medium uppercase tracking-wide opacity-70">
          {t("chat.you")}
        </p>
        {canEdit && !isEditing ? (
          <button
            aria-label={t("chat.editQuestion")}
            className="cursor-pointer rounded p-1 text-white/80 transition-colors hover:bg-white/15 hover:text-white"
            type="button"
            onClick={onStartEdit}
          >
            <svg
              aria-hidden="true"
              className="size-4"
              fill="none"
              viewBox="0 0 24 24"
            >
              <path
                d="M4 20h4l10.5-10.5a1.5 1.5 0 0 0-2.12-2.12L6 17.76V20zM14.5 6.5l3 3"
                stroke="currentColor"
                strokeLinecap="round"
                strokeLinejoin="round"
                strokeWidth="1.75"
              />
            </svg>
          </button>
        ) : null}
      </div>
      {isEditing ? (
        <form
          className="flex flex-col gap-2"
          onSubmit={(event) => {
            event.preventDefault();
            onResend();
          }}
        >
          <label className="sr-only" htmlFor={fieldId}>
            {t("form.questionLabel")}
          </label>
          <textarea
            ref={textareaRef}
            id={fieldId}
            className="min-h-20 w-full resize-y rounded-md border border-zinc-300 bg-white px-3 py-2 text-zinc-900 outline-none focus:border-zinc-500"
            name="edited-question"
            rows={3}
            value={editValue}
            onChange={(event) => onEditValueChange(event.target.value)}
            onKeyDown={(event) => {
              if (event.key === "Escape") {
                event.preventDefault();
                onCancelEdit();
              }
            }}
          />
          <div className="flex flex-wrap gap-2">
            <button
              className="cursor-pointer rounded-md bg-white px-3 py-1.5 text-sm text-zinc-900 transition-colors hover:bg-zinc-200 disabled:cursor-not-allowed disabled:bg-zinc-400 disabled:hover:bg-zinc-400"
              type="submit"
              disabled={editValue.trim().length === 0}
            >
              {t("chat.sendAgain")}
            </button>
            <button
              className="cursor-pointer rounded-md border border-white/40 px-3 py-1.5 text-sm text-white transition-colors hover:border-white/70 hover:bg-white/15"
              type="button"
              onClick={onCancelEdit}
            >
              {t("chat.cancelEdit")}
            </button>
          </div>
        </form>
      ) : (
        <p className="whitespace-pre-wrap">{content}</p>
      )}
    </>
  );
}

export function MessageList({
  messages,
  isLoading = false,
  isEditingUser = false,
  editValue = "",
  onEditValueChange,
  onStartEdit,
  onCancelEdit,
  onResend,
}: MessageListProps) {
  const { t } = useTranslation();
  const canEditUser =
    Boolean(onStartEdit && onCancelEdit && onResend && onEditValueChange) &&
    !isLoading;

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
          {message.role === "user" ? (
            <UserQuestion
              content={message.content}
              isEditing={isEditingUser}
              canEdit={canEditUser}
              editValue={editValue}
              onEditValueChange={onEditValueChange ?? (() => undefined)}
              onStartEdit={onStartEdit ?? (() => undefined)}
              onCancelEdit={onCancelEdit ?? (() => undefined)}
              onResend={onResend ?? (() => undefined)}
            />
          ) : (
            <>
              <p className="mb-1 text-xs font-medium uppercase tracking-wide opacity-70">
                M.I.N.D
              </p>
              <AssistantMarkdown content={message.content} />
              {message.prompt ? (
                <details className="mt-3 border-t border-zinc-200 pt-2">
                  <summary className="cursor-pointer rounded px-1 py-0.5 text-xs font-medium text-zinc-600 transition-colors hover:bg-zinc-100 hover:text-zinc-900">
                    {t("chat.debugPrompt")}
                  </summary>
                  <pre className="mt-2 max-h-96 overflow-auto whitespace-pre-wrap break-words text-xs text-zinc-700">
                    {message.prompt}
                  </pre>
                </details>
              ) : null}
            </>
          )}
        </li>
      ))}
      {isLoading ? <GeneratingIndicator /> : null}
    </ul>
  );
}
