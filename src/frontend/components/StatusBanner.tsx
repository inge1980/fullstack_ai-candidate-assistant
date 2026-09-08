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
  if (status === "loading") {
    return (
      <p className="text-sm text-zinc-600" role="status">
        Generating an answer from retrieved project evidence?
      </p>
    );
  }

  if (status === "error") {
    return (
      <p className="text-sm text-red-700" role="alert">
        {errorMessage ?? "Something went wrong. Try again."}
      </p>
    );
  }

  if (!hasMessages) {
    return (
      <p className="text-sm text-zinc-600">
        Ask a question about the candidate?s projects, technologies, or
        decisions. Answers come from the existing API, not from the browser.
      </p>
    );
  }

  return null;
}
