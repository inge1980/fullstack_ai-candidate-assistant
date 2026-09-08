import { useState } from "react";
import { useTranslation } from "react-i18next";
import { askQuestion } from "./client/questions";
import { QuestionForm } from "./components/QuestionForm";
import { MessageList, type ChatMessage } from "./components/MessageList";
import { StatusBanner, type ChatStatus } from "./components/StatusBanner";
import { LanguageMenu } from "./components/LanguageMenu";
import { resolveAppLocale } from "./i18n/config";

export function App() {
  const { t, i18n } = useTranslation();
  const [draft, setDraft] = useState("");
  const [messages, setMessages] = useState<ChatMessage[]>([]);
  const [status, setStatus] = useState<ChatStatus>("empty");
  const [errorMessage, setErrorMessage] = useState<string | null>(null);
  const locale = resolveAppLocale(i18n.resolvedLanguage ?? i18n.language);

  const isLoading = status === "loading";

  async function handleSubmit() {
    const question = draft.trim();
    if (question.length === 0 || isLoading) {
      return;
    }

    const userMessage: ChatMessage = {
      id: crypto.randomUUID(),
      role: "user",
      content: question,
    };

    setMessages((current) => [...current, userMessage]);
    setDraft("");
    setErrorMessage(null);
    setStatus("loading");

    try {
      const response = await askQuestion(question, locale);
      const assistantMessage: ChatMessage = {
        id: crypto.randomUUID(),
        role: "assistant",
        content: response.answer,
      };
      setMessages((current) => [...current, assistantMessage]);
      setStatus("idle");
    } catch (error) {
      const message =
        error instanceof Error ? error.message : t("status.requestFailed");
      setErrorMessage(message);
      setStatus("error");
    }
  }

  function handleReset() {
    setDraft("");
    setMessages([]);
    setErrorMessage(null);
    setStatus("empty");
  }

  return (
    <div className="mx-auto flex min-h-screen max-w-2xl flex-col gap-4 px-4 py-8">
      <header className="flex items-start justify-between gap-4">
        <div>
          <h1 className="text-xl font-semibold text-zinc-900">M.I.N.D</h1>
          <p className="text-sm text-zinc-600">{t("app.subtitle")}</p>
        </div>
        <div className="flex items-center gap-2">
          <LanguageMenu />
          <button
            className="rounded-md border border-zinc-300 px-3 py-1.5 text-sm text-zinc-800 disabled:cursor-not-allowed disabled:text-zinc-400"
            type="button"
            onClick={handleReset}
            disabled={isLoading || (messages.length === 0 && draft.length === 0)}
          >
            {t("app.clear")}
          </button>
        </div>
      </header>

      <StatusBanner
        status={status}
        errorMessage={errorMessage}
        hasMessages={messages.length > 0}
      />

      <MessageList messages={messages} />

      <QuestionForm
        value={draft}
        disabled={isLoading}
        onChange={setDraft}
        onSubmit={() => {
          void handleSubmit();
        }}
      />
    </div>
  );
}
