import { useEffect, useId, useRef } from "react";

type ConfirmDialogProps = {
  open: boolean;
  title: string;
  message: string;
  confirmLabel: string;
  cancelLabel: string;
  onConfirm: () => void;
  onCancel: () => void;
};

export function ConfirmDialog({
  open,
  title,
  message,
  confirmLabel,
  cancelLabel,
  onConfirm,
  onCancel,
}: ConfirmDialogProps) {
  const titleId = useId();
  const messageId = useId();
  const cancelRef = useRef<HTMLButtonElement>(null);

  useEffect(() => {
    if (!open) {
      return;
    }

    cancelRef.current?.focus();

    function handleKeyDown(event: KeyboardEvent) {
      if (event.key === "Escape") {
        event.preventDefault();
        onCancel();
      }
    }

    document.addEventListener("keydown", handleKeyDown);

    return () => {
      document.removeEventListener("keydown", handleKeyDown);
    };
  }, [open, onCancel]);

  if (!open) {
    return null;
  }

  return (
    <div
      className="fixed inset-0 z-30 flex items-center justify-center bg-black/40 px-4"
      role="presentation"
      onClick={onCancel}
    >
      <div
        aria-describedby={messageId}
        aria-labelledby={titleId}
        aria-modal="true"
        className="w-full max-w-md rounded-md border border-line-soft bg-surface p-4 shadow-md"
        role="alertdialog"
        onClick={(event) => event.stopPropagation()}
      >
        <h2 className="text-base font-semibold text-ink" id={titleId}>
          {title}
        </h2>
        <p className="mt-2 text-sm text-muted" id={messageId}>
          {message}
        </p>
        <div className="mt-4 flex flex-wrap justify-end gap-2">
          <button
            className="cursor-pointer rounded-md border border-line bg-surface px-4 py-2 text-ink transition-colors hover:border-line-focus hover:bg-surface-muted"
            ref={cancelRef}
            type="button"
            onClick={onCancel}
          >
            {cancelLabel}
          </button>
          <button
            className="cursor-pointer rounded-md bg-invert px-4 py-2 text-invert-fg transition-colors hover:bg-invert-hover"
            type="button"
            onClick={onConfirm}
          >
            {confirmLabel}
          </button>
        </div>
      </div>
    </div>
  );
}
