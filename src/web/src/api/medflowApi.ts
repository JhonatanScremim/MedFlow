import type {
  AuthTokenResult,
  ConversationResponse,
  CreateExamRequest,
  ExamResponse,
  MessageResponse,
  Role,
  SendMessageRequest,
  UpdateExamStatusRequest,
} from "../types/medflow";

interface AuthRequest {
  email: string;
  password: string;
}

interface RegisterRequest extends AuthRequest {
  role: Role;
}

async function requestJson<TResponse>(
  apiBaseUrl: string,
  path: string,
  options: RequestInit = {},
  accessToken?: string,
): Promise<TResponse> {
  const base = trimTrailingSlash(apiBaseUrl);
  const url = base ? `${base}${path}` : path;

  let response: Response;
  try {
    response = await fetch(url, {
      ...options,
      headers: {
        "Content-Type": "application/json",
        ...(accessToken ? { Authorization: `Bearer ${accessToken}` } : {}),
        ...options.headers,
      },
    });
  } catch {
    throw new Error(
      "Nao foi possivel conectar na API. Confirme se o backend esta rodando (dotnet watch run na pasta MedFlow.Api).",
    );
  }

  if (!response.ok) {
    throw new Error(await readErrorMessage(response));
  }

  if (response.status === 204) {
    return undefined as TResponse;
  }

  return (await response.json()) as TResponse;
}

export const medflowApi = {
  register: (apiBaseUrl: string, request: RegisterRequest) =>
    requestJson<AuthTokenResult>(apiBaseUrl, "/api/Auth/register", {
      method: "POST",
      body: JSON.stringify(request),
    }),

  login: (apiBaseUrl: string, request: AuthRequest) =>
    requestJson<AuthTokenResult>(apiBaseUrl, "/api/Auth/login", {
      method: "POST",
      body: JSON.stringify(request),
    }),

  listExams: (apiBaseUrl: string, accessToken: string) =>
    requestJson<ExamResponse[]>(apiBaseUrl, "/api/Exam", {}, accessToken),

  createExam: (
    apiBaseUrl: string,
    accessToken: string,
    request: CreateExamRequest,
  ) =>
    requestJson<ExamResponse>(
      apiBaseUrl,
      "/api/Exam",
      {
        method: "POST",
        body: JSON.stringify(request),
      },
      accessToken,
    ),

  updateExamStatus: (
    apiBaseUrl: string,
    accessToken: string,
    examId: string,
    request: UpdateExamStatusRequest,
  ) =>
    requestJson<ExamResponse>(
      apiBaseUrl,
      `/api/Exam/${examId}/status`,
      {
        method: "PUT",
        body: JSON.stringify(request),
      },
      accessToken,
    ),

  listConversations: (apiBaseUrl: string, accessToken: string) =>
    requestJson<ConversationResponse[]>(
      apiBaseUrl,
      "/api/Conversations",
      {},
      accessToken,
    ),

  listMessages: (
    apiBaseUrl: string,
    accessToken: string,
    conversationId: string,
    page = 1,
    pageSize = 50,
  ) =>
    requestJson<MessageResponse[]>(
      apiBaseUrl,
      `/api/Conversations/${conversationId}/messages?page=${page}&pageSize=${pageSize}`,
      {},
      accessToken,
    ),

  sendMessage: (
    apiBaseUrl: string,
    accessToken: string,
    conversationId: string,
    request: SendMessageRequest,
  ) =>
    requestJson<MessageResponse>(
      apiBaseUrl,
      `/api/Conversations/${conversationId}/messages`,
      {
        method: "POST",
        body: JSON.stringify(request),
      },
      accessToken,
    ),
};

export function trimTrailingSlash(value: string): string {
  return value.replace(/\/+$/, "");
}

async function readErrorMessage(response: Response): Promise<string> {
  const fallback = `${response.status} ${response.statusText}`;

  try {
    const body = (await response.json()) as { message?: string; error?: string };
    return body.message ?? body.error ?? fallback;
  } catch {
    return fallback;
  }
}
