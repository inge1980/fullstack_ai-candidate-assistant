import { useTranslation } from "react-i18next";
import type { QuestionIntent } from "../client/types";

type QuestionFormProps = {
  value: string;
  disabled: boolean;
  intent: QuestionIntent | null;
  onChange: (value: string) => void;
  onSubmit: () => void;
};

export function formatIntentDebug(intent: QuestionIntent | null): string {
  if (!intent) {
    return "debug intent: -";
  }

  const count =
    intent.requestedCount == null ? "" : ` | n=${intent.requestedCount}`;

  return `debug intent: ${intent.category}${count}`;
}

export function QuestionForm({
  value,
  disabled,
  intent,
  onChange,
  onSubmit,
}: QuestionFormProps) {
  const { t } = useTranslation();

  return (
    <div className="flex flex-col gap-1">
      <form
        className="flex gap-2"
        onSubmit={(event) => {
          event.preventDefault();
          onSubmit();
        }}
      >
        <label className="sr-only" htmlFor="question">
          {t("form.questionLabel")}
        </label>
        <input
          id="question"
          className="min-w-0 flex-1 rounded-md border border-line bg-surface px-3 py-2 text-ink outline-none focus:border-line-focus"
          type="text"
          name="question"
          autoComplete="off"
          placeholder={t("form.placeholder")}
          value={value}
          disabled={disabled}
          onChange={(event) => onChange(event.target.value)}
        />
        <button
          className="cursor-pointer rounded-md bg-invert px-4 py-2 text-invert-fg transition-colors hover:bg-invert-hover disabled:cursor-not-allowed disabled:bg-invert-disabled disabled:hover:bg-invert-disabled"
          type="submit"
          disabled={disabled || value.trim().length === 0}
        >
          {t("form.send")}
        </button>
      </form>
      <p className="text-xs text-muted" aria-live="polite">
        {formatIntentDebug(intent)}
      </p>
    </div>
  );
}
