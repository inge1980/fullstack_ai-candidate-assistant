import { useEffect, useRef, useState } from "react";
import { useTranslation } from "react-i18next";
import { askQuestion, previewQuestionIntent } from "./client/questions";
import type { QuestionIntent } from "./client/types";
import { formatIntentDebug, QuestionForm } from "./components/QuestionForm";
import { MessageList, type ChatMessage } from "./components/MessageList";
import { StatusBanner, type ChatStatus } from "./components/StatusBanner";
import { LanguageMenu } from "./components/LanguageMenu";
import { ChatHistory } from "./components/ChatHistory";
import {
  persistChatHistory,
  readChatHistory,
  upsertChatHistory,
  type ChatHistoryItem,
} from "./chatHistory";
import { resolveAppLocale } from "./i18n/config";

export function App() {
  const { t, i18n } = useTranslation();
  const [draft, setDraft] = useState("");
  const [messages, setMessages] = useState<ChatMessage[]>([]);
  const [status, setStatus] = useState<ChatStatus>("empty");
  const [errorMessage, setErrorMessage] = useState<string | null>(null);
  const [isEditing, setIsEditing] = useState(false);
  const [editDraft, setEditDraft] = useState("");
  const [intent, setIntent] = useState<QuestionIntent | null>(null);
  const [history, setHistory] = useState<ChatHistoryItem[]>(readChatHistory);
  const askGenerationRef = useRef(0);
  const intentPreviewRef = useRef(0);
  const locale = resolveAppLocale(i18n.resolvedLanguage ?? i18n.language);

  const isLoading = status === "loading";
  const needsLanguageReset =
    draft.trim().length > 0 ||
    editDraft.trim().length > 0 ||
    messages.length > 0 ||
    isEditing ||
    isLoading ||
    errorMessage !== null;

  const previousQuestion =
    messages.find((message) => message.role === "user")?.content ?? "";

  useEffect(() => {
    if (messages.length > 0 || isLoading) {
      return;
    }

    const trimmed = draft.trim();
    if (trimmed.length === 0) {
      setIntent(null);
      return;
    }

    const previewId = ++intentPreviewRef.current;
    const handle = window.setTimeout(() => {
      void previewQuestionIntent(trimmed)
        .then((result) => {
          if (previewId === intentPreviewRef.current) {
            setIntent(result);
          }
        })
        .catch(() => {
          if (previewId === intentPreviewRef.current) {
            setIntent(null);
          }
        });
    }, 300);

    return () => {
      window.clearTimeout(handle);
    };
  }, [draft, isLoading, messages.length]);

  async function ask(question: string) {
    const trimmed = question.trim();
    if (trimmed.length === 0 || isLoading) {
      return;
    }

    const generation = ++askGenerationRef.current;

    const userMessage: ChatMessage = {
      id: crypto.randomUUID(),
      role: "user",
      content: trimmed,
    };

    setMessages([userMessage]);
    setDraft("");
    setEditDraft("");
    setIsEditing(false);
    setErrorMessage(null);
    setStatus("loading");

    try {
      const response = await askQuestion(trimmed, locale);
      if (generation !== askGenerationRef.current) {
        return;
      }

      const assistantMessage: ChatMessage = {
        id: crypto.randomUUID(),
        role: "assistant",
        content: response.answer,
        prompt: response.prompt ?? undefined,
      };
      setMessages((current) => [...current, assistantMessage]);
      setHistory((current) => {
        const next = upsertChatHistory(current, {
          question: trimmed,
          answer: response.answer,
          prompt: response.prompt ?? undefined,
        });
        persistChatHistory(next);
        return next;
      });
      setIntent(response.intent ?? null);
      setStatus("idle");
    } catch (error) {
      if (generation !== askGenerationRef.current) {
        return;
      }

      const message =
        error instanceof Error ? error.message : t("status.requestFailed");
      setErrorMessage(message);
      setStatus("error");
    }
  }

  function handleReset() {
    askGenerationRef.current += 1;
    setDraft("");
    setEditDraft("");
    setIsEditing(false);
    setMessages([]);
    setErrorMessage(null);
    setIntent(null);
    intentPreviewRef.current += 1;
    setStatus("empty");
  }

  function handleViewHistory(item: ChatHistoryItem) {
    askGenerationRef.current += 1;
    setDraft("");
    setEditDraft("");
    setIsEditing(false);
    setErrorMessage(null);
    setIntent(null);
    intentPreviewRef.current += 1;
    setStatus("idle");
    setMessages([
      {
        id: `${item.id}-user`,
        role: "user",
        content: item.question,
      },
      {
        id: `${item.id}-assistant`,
        role: "assistant",
        content: item.answer,
        prompt: item.prompt,
      },
    ]);
  }

  function handleClearHistory() {
    setHistory([]);
    persistChatHistory([]);
  }

  return (
    <div className="mx-auto flex min-h-screen max-w-5xl flex-col gap-6 px-4 py-8 md:flex-row md:items-start">
      <ChatHistory
        items={history}
        activeQuestion={previousQuestion}
        disabled={isLoading}
        onView={handleViewHistory}
        onReask={(question) => {
          void ask(question);
        }}
        onClear={handleClearHistory}
      />

      <div className="flex min-w-0 flex-1 flex-col gap-4">
        <header className="flex items-start justify-between gap-4">
          <div>
            <h1 className="text-xl font-semibold text-ink">M.I.N.D</h1>
            <p className="text-sm text-muted">{t("app.subtitle")}</p>
          </div>
          <LanguageMenu needsReset={needsLanguageReset} onReset={handleReset} />
        </header>

        <StatusBanner
          status={status}
          errorMessage={errorMessage}
          hasMessages={messages.length > 0}
        />

        <MessageList
          messages={messages}
          isLoading={isLoading}
          isEditingUser={isEditing}
          editValue={editDraft}
          onEditValueChange={setEditDraft}
          onStartEdit={() => {
            setEditDraft(previousQuestion);
            setIsEditing(true);
          }}
          onCancelEdit={() => {
            setIsEditing(false);
            setEditDraft("");
          }}
          onResend={() => {
            void ask(editDraft);
          }}
        />

        {messages.length === 0 ? (
          <QuestionForm
            value={draft}
            disabled={isLoading}
            intent={intent}
            onChange={setDraft}
            onSubmit={() => {
              void ask(draft);
            }}
          />
        ) : (
          <p className="text-xs text-muted" aria-live="polite">
            {formatIntentDebug(intent)}
          </p>
        )}

        {messages.length > 0 && !isLoading ? (
          <div className="flex flex-wrap gap-2">
            <button
              className="cursor-pointer rounded-md bg-invert px-4 py-2 text-invert-fg transition-colors hover:bg-invert-hover"
              type="button"
              onClick={handleReset}
            >
              {t("app.askNewQuestion")}
            </button>
            {isEditing ? null : (
              <button
                className="cursor-pointer rounded-md border border-line bg-surface px-4 py-2 text-ink transition-colors hover:border-line-focus hover:bg-surface-muted"
                type="button"
                onClick={() => {
                  void ask(previousQuestion);
                }}
              >
                {t("app.askSameQuestion")}
              </button>
            )}
          </div>
        ) : null}
      </div>
    </div>
  );
}
