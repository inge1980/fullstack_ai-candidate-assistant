import { useTranslation } from "react-i18next";

export type ChatStatus = "idle" | "loading" | "error" | "empty";

type StatusBannerProps = {
  status: ChatStatus;
  errorMessage: string | null;
  hasMessages: boolean;
};

export function StatusBanner({
  status,
  errorMessage,
  hasMessages,
}: StatusBannerProps) {
  const { t } = useTranslation();

  if (status === "loading") {
    return null;
  }

  if (status === "error") {
    return (
      <p className="text-sm text-red-700" role="alert">
        {errorMessage ?? t("status.errorFallback")}
      </p>
    );
  }

  if (!hasMessages) {
    return <p className="text-sm text-zinc-600">{t("status.empty")}</p>;
  }

  return null;
}
