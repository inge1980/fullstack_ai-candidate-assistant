import { useCallback, useState } from "react";
import { useTranslation } from "react-i18next";
import type { ChatHistoryItem } from "../chatHistory";
import { ConfirmDialog } from "./ConfirmDialog";

type ChatHistoryProps = {
  items: ChatHistoryItem[];
  activeQuestion: string;
  disabled?: boolean;
  onView: (item: ChatHistoryItem) => void;
  onReask: (question: string) => void;
  onClear: () => void;
};

export function ChatHistory({
  items,
  activeQuestion,
  disabled = false,
  onView,
  onReask,
  onClear,
}: ChatHistoryProps) {
  const { t } = useTranslation();
  const [confirmOpen, setConfirmOpen] = useState(false);
  const canClear = items.length > 0 && !disabled;

  const cancelClear = useCallback(() => {
    setConfirmOpen(false);
  }, []);

  function confirmClear() {
    setConfirmOpen(false);
    onClear();
  }

  return (
    <aside className="flex w-full shrink-0 flex-col gap-3 md:sticky md:top-8 md:w-64 lg:w-72">
      <div className="flex items-center justify-between gap-2">
        <h2 className="text-sm font-semibold text-ink">{t("history.title")}</h2>
        <button
          className="cursor-pointer rounded-md border border-line bg-surface px-2 py-1 text-xs text-ink transition-colors hover:border-line-focus hover:bg-surface-muted disabled:cursor-not-allowed disabled:opacity-50"
          type="button"
          disabled={!canClear}
          onClick={() => setConfirmOpen(true)}
        >
          {t("history.clear")}
        </button>
      </div>

      {items.length === 0 ? (
        <p className="rounded-md border border-line-soft bg-surface px-3 py-2 text-sm text-muted">
          {t("history.empty")}
        </p>
      ) : (
        <ul className="flex max-h-[40vh] flex-col gap-1 overflow-y-auto md:max-h-[calc(100vh-8rem)]">
          {items.map((item) => {
            const isActive = item.question === activeQuestion;

            return (
              <li
                key={item.id}
                className={`flex items-start gap-1 rounded-md border px-2 py-1.5 ${
                  isActive
                    ? "border-line bg-surface-muted"
                    : "border-line-soft bg-surface"
                }`}
              >
                <p className="min-w-0 flex-1 truncate text-sm text-ink" title={item.question}>
                  {item.question}
                </p>
                <div className="flex shrink-0 gap-0.5">
                  <button
                    aria-label={t("history.view")}
                    className="cursor-pointer rounded p-1 text-muted transition-colors hover:bg-surface-muted hover:text-ink disabled:cursor-not-allowed disabled:opacity-50"
                    title={t("history.view")}
                    type="button"
                    disabled={disabled}
                    onClick={() => onView(item)}
                  >
                    <svg
                      aria-hidden="true"
                      className="size-4"
                      fill="none"
                      viewBox="0 0 24 24"
                    >
                      <path
                        d="M2.5 12s3.5-7 9.5-7 9.5 7 9.5 7-3.5 7-9.5 7-9.5-7-9.5-7z"
                        stroke="currentColor"
                        strokeWidth="1.75"
                      />
                      <circle
                        cx="12"
                        cy="12"
                        r="2.5"
                        stroke="currentColor"
                        strokeWidth="1.75"
                      />
                    </svg>
                  </button>
                  <button
                    aria-label={t("history.reask")}
                    className="cursor-pointer rounded p-1 text-muted transition-colors hover:bg-surface-muted hover:text-ink disabled:cursor-not-allowed disabled:opacity-50"
                    title={t("history.reask")}
                    type="button"
                    disabled={disabled}
                    onClick={() => onReask(item.question)}
                  >
                    <svg
                      aria-hidden="true"
                      className="size-4"
                      fill="none"
                      viewBox="0 0 24 24"
                    >
                      <path
                        d="M20 12a8 8 0 1 1-2.2-5.5M20 4v4h-4"
                        stroke="currentColor"
                        strokeLinecap="round"
                        strokeLinejoin="round"
                        strokeWidth="1.75"
                      />
                    </svg>
                  </button>
                </div>
              </li>
            );
          })}
        </ul>
      )}

      <ConfirmDialog
        open={confirmOpen}
        title={t("history.clearTitle")}
        message={t("history.clearWarning")}
        confirmLabel={t("history.clearConfirm")}
        cancelLabel={t("history.clearCancel")}
        onConfirm={confirmClear}
        onCancel={cancelClear}
      />
    </aside>
  );
}
