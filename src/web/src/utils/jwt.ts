import type { JwtProfile } from "../types/medflow";

interface JwtPayload {
  sub?: string;
  email?: string;
  roles?: string;
  role?: string | string[];
  doctorId?: string;
  patientId?: string;
  "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"?:
    | string
    | string[];
}

export function decodeJwtProfile(token: string): JwtProfile {
  const [, payload] = token.split(".");

  if (!payload) {
    return { roles: [] };
  }

  try {
    const normalizedPayload = payload.replace(/-/g, "+").replace(/_/g, "/");
    const json = decodeURIComponent(
      atob(normalizedPayload)
        .split("")
        .map((char) => `%${`00${char.charCodeAt(0).toString(16)}`.slice(-2)}`)
        .join(""),
    );
    const parsed = JSON.parse(json) as JwtPayload;

    return {
      userId: parsed.sub,
      email: parsed.email,
      roles: normalizeRoles(parsed),
      doctorId: parsed.doctorId,
      patientId: parsed.patientId,
    };
  } catch {
    return { roles: [] };
  }
}

function normalizeRoles(payload: JwtPayload): string[] {
  const roleClaim =
    payload["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"] ??
    payload.role;

  if (Array.isArray(roleClaim)) {
    return roleClaim;
  }

  const roles = [
    ...(payload.roles?.split(",") ?? []),
    ...(roleClaim ? [roleClaim] : []),
  ];

  return [...new Set(roles.map((role) => role.trim()).filter(Boolean))];
}
