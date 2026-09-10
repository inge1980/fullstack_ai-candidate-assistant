const STORAGE_KEY = "mind-chat-history";
const MAX_ITEMS = 50;

export type ChatHistoryItem = {
  id: string;
  question: string;
  answer: string;
  prompt?: string;
  askedAt: number;
};

function isHistoryItem(value: unknown): value is ChatHistoryItem {
  if (typeof value !== "object" || value === null) {
    return false;
  }

  const item = value as Partial<ChatHistoryItem>;
  return (
    typeof item.id === "string" &&
    typeof item.question === "string" &&
    typeof item.answer === "string" &&
    typeof item.askedAt === "number" &&
    (item.prompt === undefined || typeof item.prompt === "string")
  );
}

export function readChatHistory(): ChatHistoryItem[] {
  try {
    const raw = localStorage.getItem(STORAGE_KEY);
    if (!raw) {
      return [];
    }

    const parsed: unknown = JSON.parse(raw);
    if (!Array.isArray(parsed)) {
      return [];
    }

    return parsed.filter(isHistoryItem);
  } catch {
    return [];
  }
}

export function persistChatHistory(items: ChatHistoryItem[]): void {
  try {
    localStorage.setItem(STORAGE_KEY, JSON.stringify(items));
  } catch {
    // Quota or private-mode failures should not break the chat.
  }
}

export function upsertChatHistory(
  items: ChatHistoryItem[],
  entry: Omit<ChatHistoryItem, "id" | "askedAt"> & { id?: string },
): ChatHistoryItem[] {
  const question = entry.question.trim();
  const existing = items.find((item) => item.question === question);
  const nextItem: ChatHistoryItem = {
    id: existing?.id ?? entry.id ?? crypto.randomUUID(),
    question,
    answer: entry.answer,
    prompt: entry.prompt,
    askedAt: Date.now(),
  };

  return [nextItem, ...items.filter((item) => item.id !== nextItem.id)].slice(
    0,
    MAX_ITEMS,
  );
}
