import { useEffect, useId, useRef, useState } from "react";
import { useTranslation } from "react-i18next";
import { NO, US } from "country-flag-icons/react/3x2";
import { resolveAppLocale, type AppLocale } from "../i18n/config";

const options: {
  locale: AppLocale;
  Flag: typeof US;
  codeKey: "language.codeUs" | "language.codeNb";
  nameKey: "language.englishUs" | "language.norwegianNb";
}[] = [
  {
    locale: "us",
    Flag: US,
    codeKey: "language.codeUs",
    nameKey: "language.englishUs",
  },
  {
    locale: "nb",
    Flag: NO,
    codeKey: "language.codeNb",
    nameKey: "language.norwegianNb",
  },
];

export function LanguageMenu() {
  const { t, i18n } = useTranslation();
  const [open, setOpen] = useState(false);
  const rootRef = useRef<HTMLDivElement>(null);
  const menuId = useId();
  const locale = resolveAppLocale(i18n.resolvedLanguage ?? i18n.language);
  const current = options.find((option) => option.locale === locale) ?? options[0];
  const CurrentFlag = current.Flag;
  const currentName = t(current.nameKey);

  useEffect(() => {
    if (!open) {
      return;
    }

    function handlePointerDown(event: PointerEvent) {
      if (rootRef.current?.contains(event.target as Node)) {
        return;
      }

      setOpen(false);
    }

    function handleKeyDown(event: KeyboardEvent) {
      if (event.key === "Escape") {
        setOpen(false);
      }
    }

    document.addEventListener("pointerdown", handlePointerDown);
    document.addEventListener("keydown", handleKeyDown);

    return () => {
      document.removeEventListener("pointerdown", handlePointerDown);
      document.removeEventListener("keydown", handleKeyDown);
    };
  }, [open]);

  async function selectLocale(next: AppLocale) {
    setOpen(false);

    if (next !== locale) {
      await i18n.changeLanguage(next);
    }
  }

  return (
    <div className="relative" ref={rootRef}>
      <button
        aria-controls={menuId}
        aria-expanded={open}
        aria-haspopup="listbox"
        aria-label={currentName}
        className="flex items-center gap-1 rounded-md border border-zinc-300 px-1.5 py-1.5 text-zinc-800 hover:bg-zinc-100"
        type="button"
        onClick={() => setOpen((value) => !value)}
      >
        <CurrentFlag aria-hidden="true" className="h-5 w-7 rounded-sm" />
        <svg
          aria-hidden="true"
          className={`h-3.5 w-3.5 shrink-0 text-zinc-600 transition-transform ${
            open ? "rotate-180" : ""
          }`}
          fill="none"
          viewBox="0 0 16 16"
        >
          <path
            d="M4 6l4 4 4-4"
            stroke="currentColor"
            strokeLinecap="round"
            strokeLinejoin="round"
            strokeWidth="1.5"
          />
        </svg>
        <span className="sr-only">{currentName}</span>
      </button>
      {open ? (
        <ul
          className="absolute right-0 z-10 mt-1 min-w-[9rem] rounded-md border border-zinc-200 bg-white py-1 shadow-md"
          id={menuId}
          role="listbox"
          aria-label={t("language.choose")}
        >
          {options.map((option) => {
            const OptionFlag = option.Flag;
            const selected = option.locale === locale;
            const name = t(option.nameKey);

            return (
              <li key={option.locale} role="option" aria-selected={selected}>
                <button
                  className={`flex w-full items-center gap-2 px-3 py-1.5 text-left text-sm text-zinc-800 hover:bg-zinc-100 ${
                    selected ? "bg-zinc-50 font-medium" : ""
                  }`}
                  type="button"
                  aria-label={name}
                  onClick={() => {
                    void selectLocale(option.locale);
                  }}
                >
                  <OptionFlag aria-hidden="true" className="h-4 w-6 rounded-sm" />
                  <span aria-hidden="true">{t(option.codeKey)}</span>
                  <span className="sr-only">{name}</span>
                </button>
              </li>
            );
          })}
        </ul>
      ) : null}
    </div>
  );
}
