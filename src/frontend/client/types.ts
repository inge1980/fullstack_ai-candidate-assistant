/** Mirrors Application.Questions.AskQuestionRequest */
export type AskQuestionRequest = {
  question: string;
  locale: "us" | "nb";
};

/** Mirrors Application.Questions.QuestionRelevance */
export type QuestionRelevance = {
  combined: number;
  vector: number;
  metadata: number;
  evidence: number;
};

/** Mirrors Application.Questions.QuestionSource */
export type QuestionSource = {
  projectId: string;
  title: string;
  url: string | null;
  heading: string | null;
  semanticType: string | null;
  content: string | null;
  source: string | null;
  relevance: QuestionRelevance | null;
};

/** Mirrors Application.Questions.QuestionIntent */
export type QuestionIntent = {
  category: string;
  requestedCount: number | null;
};

/** Mirrors Application.Questions.AskQuestionResponse */
export type AskQuestionResponse = {
  answer: string;
  sources: QuestionSource[];
  prompt?: string | null;
  intent?: QuestionIntent | null;
};

/** ASP.NET Core ProblemDetails (400 validation) */
export type ProblemDetails = {
  title?: string;
  detail?: string;
  status?: number;
};
