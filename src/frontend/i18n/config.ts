import i18n from "i18next";
import { initReactI18next } from "react-i18next";
import { nb } from "./locales/nb";
import { us } from "./locales/us";

export const appLocales = ["us", "nb"] as const;

export type AppLocale = (typeof appLocales)[number];

const STORAGE_KEY = "mind-locale";

export function isAppLocale(value: string): value is AppLocale {
  return value === "us" || value === "nb";
}

export function resolveAppLocale(language: string | undefined): AppLocale {
  return language && isAppLocale(language) ? language : "us";
}

export function htmlLangFor(locale: AppLocale): string {
  return locale === "nb" ? "nb" : "en";
}

export function readStoredLocale(): AppLocale {
  const stored = localStorage.getItem(STORAGE_KEY);
  return stored && isAppLocale(stored) ? stored : "us";
}

export function persistLocale(locale: AppLocale): void {
  localStorage.setItem(STORAGE_KEY, locale);
  document.documentElement.lang = htmlLangFor(locale);
}

void i18n.use(initReactI18next).init({
  resources: {
    us: { translation: us },
    nb: { translation: nb },
  },
  lng: readStoredLocale(),
  fallbackLng: "us",
  interpolation: {
    escapeValue: false,
  },
  react: {
    useSuspense: false,
  },
});

persistLocale(readStoredLocale());

i18n.on("languageChanged", (language) => {
  persistLocale(isAppLocale(language) ? language : "us");
});

export { i18n };
