export type Role = "Patient" | "Doctor";

export type ConnectionStatus =
  | "disconnected"
  | "connecting"
  | "connected"
  | "reconnecting";

export type ExamType = 1 | 2 | 3 | 4 | 5 | 6;

export type ExamStatus = 0 | 1 | 2 | 3;

export interface AuthTokenResult {
  accessToken: string;
  expiresAtUtc: string;
}

export interface JwtProfile {
  userId?: string;
  email?: string;
  roles: string[];
  doctorId?: string;
  patientId?: string;
}

export interface SessionState {
  token: string;
  expiresAtUtc: string;
  profile: JwtProfile;
}

export interface CreateExamRequest {
  type: ExamType;
  doctorId?: string;
  scheduledAtUtc?: string;
  notes?: string;
}

export interface UpdateExamStatusRequest {
  status: ExamStatus;
}

export interface ExamResponse {
  id: string;
  patientId: string;
  doctorId?: string | null;
  conversationId?: string | null;
  type: ExamType;
  status: ExamStatus;
  scheduledAtUtc?: string | null;
  notes?: string | null;
  createdAt: string;
  updatedAt: string;
}

export interface ConversationResponse {
  id: string;
  examId: string;
  patientId: string;
  doctorId?: string | null;
  createdAt: string;
}

export interface MessageResponse {
  id: string;
  conversationId: string;
  senderUserId?: string | null;
  content: string;
  sentAt: string;
}

export interface SendMessageRequest {
  content: string;
}

export interface RealtimeEvent {
  id: string;
  at: string;
  type: "ExamCreated" | "ExamUpdated" | "ReceiveMessage" | "System";
  payload: unknown;
}

export const examTypeLabels: Record<ExamType, string> = {
  1: "BloodTest",
  2: "XRay",
  3: "Ultrasound",
  4: "MagneticResonanceImaging",
  5: "ComputedTomography",
  6: "Electrocardiogram",
};

export const examStatusLabels: Record<ExamStatus, string> = {
  0: "Requested",
  1: "InProgress",
  2: "Completed",
  3: "Cancelled",
};
