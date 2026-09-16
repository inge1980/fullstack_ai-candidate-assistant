import type {
  AskQuestionRequest,
  AskQuestionResponse,
  ProblemDetails,
  QuestionIntent,
  QuestionProgress,
} from "./types";
import { isQuestionPhase } from "./types";

export class ApiError extends Error {
  readonly status: number;

  constructor(message: string, status: number) {
    super(message);
    this.name = "ApiError";
    this.status = status;
  }
}

export async function askQuestion(
  question: string,
  locale: AskQuestionRequest["locale"],
  onPhase?: (progress: QuestionProgress) => void,
): Promise<AskQuestionResponse> {
  const body: AskQuestionRequest = { question, locale };

  const url = import.meta.env.DEV
    ? "/api/v1/Questions/progress?includeDebug=true"
    : "/api/v1/Questions/progress";

  const response = await fetch(url, {
    method: "POST",
    headers: {
      Accept: "text/event-stream",
      "Content-Type": "application/json",
    },
    body: JSON.stringify(body),
  });

  if (!response.ok) {
    throw new ApiError(await readErrorMessage(response), response.status);
  }

  return readQuestionProgress(response, onPhase);
}

export async function previewQuestionIntent(
  question: string,
): Promise<QuestionIntent> {
  const body: Pick<AskQuestionRequest, "question"> = { question };

  const response = await fetch("/api/v1/Questions/intent", {
    method: "POST",
    headers: {
      Accept: "application/json",
      "Content-Type": "application/json",
    },
    body: JSON.stringify(body),
  });

  if (!response.ok) {
    throw new ApiError(await readErrorMessage(response), response.status);
  }

  return (await response.json()) as QuestionIntent;
}

async function readQuestionProgress(
  response: Response,
  onPhase?: (progress: QuestionProgress) => void,
): Promise<AskQuestionResponse> {
  if (!response.body) {
    throw new ApiError("Request failed.", response.status);
  }

  const reader = response.body.getReader();
  const decoder = new TextDecoder();
  let buffer = "";
  let result: AskQuestionResponse | undefined;
  let errorDetail: string | undefined;

  while (true) {
    const { done, value } = await reader.read();
    if (done) {
      break;
    }

    buffer += decoder.decode(value, { stream: true });
    const parsed = consumeSseFrames(buffer);
    buffer = parsed.rest;

    for (const frame of parsed.frames) {
      if (frame.event === "phase") {
        const progress = readPhaseProgress(frame.data);
        if (progress) {
          onPhase?.(progress);
        }
        continue;
      }

      if (frame.event === "result") {
        result = JSON.parse(frame.data) as AskQuestionResponse;
        continue;
      }

      if (frame.event === "error") {
        errorDetail = readSseErrorDetail(frame.data);
      }
    }
  }

  if (errorDetail) {
    throw new ApiError(errorDetail, 500);
  }

  if (!result) {
    throw new ApiError("Request failed.", response.status);
  }

  return result;
}

function consumeSseFrames(buffer: string): {
  frames: { event: string; data: string }[];
  rest: string;
} {
  const frames: { event: string; data: string }[] = [];
  const chunks = buffer.split(/\r?\n\r?\n/);
  const rest = chunks.pop() ?? "";

  for (const chunk of chunks) {
    if (chunk.trim().length === 0) {
      continue;
    }

    let event = "message";
    const dataLines: string[] = [];

    for (const line of chunk.split(/\r?\n/)) {
      if (line.startsWith("event:")) {
        event = line.slice("event:".length).trim();
        continue;
      }

      if (line.startsWith("data:")) {
        dataLines.push(line.slice("data:".length).trimStart());
      }
    }

    frames.push({ event, data: dataLines.join("\n") });
  }

  return { frames, rest };
}

function readPhaseProgress(data: string): QuestionProgress | null {
  try {
    const payload = JSON.parse(data) as {
      phase?: string;
      provider?: string | null;
      model?: string | null;
    };

    if (payload.phase && isQuestionPhase(payload.phase)) {
      return {
        phase: payload.phase,
        provider: payload.provider,
        model: payload.model,
      };
    }
  } catch {
    if (isQuestionPhase(data)) {
      return { phase: data };
    }
  }

  return null;
}

function readSseErrorDetail(data: string): string {
  try {
    const payload = JSON.parse(data) as ProblemDetails;
    return payload.detail ?? payload.title ?? "Request failed.";
  } catch {
    return data.length > 0 ? data : "Request failed.";
  }
}

async function readErrorMessage(response: Response): Promise<string> {
  try {
    const payload = (await response.json()) as ProblemDetails;
    return payload.detail ?? payload.title ?? `Request failed (${response.status})`;
  } catch {
    return `Request failed (${response.status})`;
  }
}
