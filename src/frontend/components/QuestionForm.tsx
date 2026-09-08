import { useTranslation } from "react-i18next";

type QuestionFormProps = {
  value: string;
  disabled: boolean;
  onChange: (value: string) => void;
  onSubmit: () => void;
};

export function QuestionForm({
  value,
  disabled,
  onChange,
  onSubmit,
}: QuestionFormProps) {
  const { t } = useTranslation();

  return (
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
        className="min-w-0 flex-1 rounded-md border border-zinc-300 bg-white px-3 py-2 text-zinc-900 outline-none focus:border-zinc-500"
        type="text"
        name="question"
        autoComplete="off"
        placeholder={t("form.placeholder")}
        value={value}
        disabled={disabled}
        onChange={(event) => onChange(event.target.value)}
      />
      <button
        className="rounded-md bg-zinc-900 px-4 py-2 text-white disabled:cursor-not-allowed disabled:bg-zinc-400"
        type="submit"
        disabled={disabled || value.trim().length === 0}
      >
        {t("form.send")}
      </button>
    </form>
  );
}
