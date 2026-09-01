import type { SessionState } from "../types/medflow";

export function isSessionExpired(session: SessionState | null): boolean {
  if (!session?.expiresAtUtc) {
    return true;
  }

  return new Date(session.expiresAtUtc).getTime() <= Date.now();
}
