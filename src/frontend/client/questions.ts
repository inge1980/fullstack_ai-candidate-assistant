import type {
  AskQuestionRequest,
  AskQuestionResponse,
  ProblemDetails,
} from "./types";

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
): Promise<AskQuestionResponse> {
  const body: AskQuestionRequest = { question, locale };

  const url = import.meta.env.DEV
    ? "/api/v1/Questions?includeDebug=true"
    : "/api/v1/Questions";

  const response = await fetch(url, {
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

  return (await response.json()) as AskQuestionResponse;
}

async function readErrorMessage(response: Response): Promise<string> {
  try {
    const payload = (await response.json()) as ProblemDetails;
    return payload.detail ?? payload.title ?? `Request failed (${response.status})`;
  } catch {
    return `Request failed (${response.status})`;
  }
}
