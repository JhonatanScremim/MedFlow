import {
  createContext,
  useCallback,
  useContext,
  useEffect,
  useMemo,
  useState,
  type ReactNode,
} from "react";
import { medflowApi } from "../api/medflowApi";
import type { Role, SessionState } from "../types/medflow";
import { decodeJwtProfile } from "../utils/jwt";

const SESSION_STORAGE_KEY = "medflow.web.session";
const LEGACY_SESSIONS_KEY = "medflow.web.sessions";
const API_BASE_URL_KEY = "medflow.web.apiBaseUrl";

function loadApiBaseUrl(): string {
  const fromEnv = import.meta.env.VITE_MEDFLOW_API_URL;
  if (fromEnv !== undefined && fromEnv !== "") {
    return fromEnv;
  }

  if (import.meta.env.DEV) {
    return "";
  }

  const stored = localStorage.getItem(API_BASE_URL_KEY);
  if (stored && !stored.includes("localhost:7026")) {
    return stored;
  }

  return "http://localhost:5113";
}

interface AuthContextValue {
  session: SessionState | null;
  apiBaseUrl: string;
  role: Role | null;
  setApiBaseUrl: (url: string) => void;
  login: (email: string, password: string) => Promise<Role>;
  register: (email: string, password: string, role: Role) => Promise<Role>;
  logout: () => void;
}

const AuthContext = createContext<AuthContextValue | null>(null);

function loadSession(): SessionState | null {
  const raw = localStorage.getItem(SESSION_STORAGE_KEY);
  if (raw) {
    try {
      return JSON.parse(raw) as SessionState;
    } catch {
      return null;
    }
  }

  const legacy = localStorage.getItem(LEGACY_SESSIONS_KEY);
  if (!legacy) {
    return null;
  }

  try {
    const parsed = JSON.parse(legacy) as {
      Patient?: SessionState | null;
      Doctor?: SessionState | null;
    };
    return parsed.Patient ?? parsed.Doctor ?? null;
  } catch {
    return null;
  }
}

function resolveRole(session: SessionState | null): Role | null {
  if (!session) {
    return null;
  }

  if (session.profile.roles.includes("Doctor")) {
    return "Doctor";
  }

  if (session.profile.roles.includes("Patient")) {
    return "Patient";
  }

  return null;
}

function buildSession(accessToken: string, expiresAtUtc: string): SessionState {
  return {
    token: accessToken,
    expiresAtUtc,
    profile: decodeJwtProfile(accessToken),
  };
}

export function AuthProvider({ children }: { children: ReactNode }) {
  const [session, setSession] = useState<SessionState | null>(loadSession);
  const [apiBaseUrl, setApiBaseUrlState] = useState(loadApiBaseUrl);

  const role = useMemo(() => resolveRole(session), [session]);

  useEffect(() => {
    if (apiBaseUrl) {
      localStorage.setItem(API_BASE_URL_KEY, apiBaseUrl);
    } else {
      localStorage.removeItem(API_BASE_URL_KEY);
    }
  }, [apiBaseUrl]);

  useEffect(() => {
    if (session) {
      localStorage.setItem(SESSION_STORAGE_KEY, JSON.stringify(session));
    } else {
      localStorage.removeItem(SESSION_STORAGE_KEY);
    }
  }, [session]);

  const persistAuth = useCallback((accessToken: string, expiresAtUtc: string) => {
    const nextSession = buildSession(accessToken, expiresAtUtc);
    setSession(nextSession);
    const nextRole = resolveRole(nextSession);
    if (!nextRole) {
      throw new Error("Token sem role Patient ou Doctor.");
    }
    return nextRole;
  }, []);

  const login = useCallback(
    async (email: string, password: string) => {
      const result = await medflowApi.login(apiBaseUrl, { email, password });
      return persistAuth(result.accessToken, result.expiresAtUtc);
    },
    [apiBaseUrl, persistAuth],
  );

  const register = useCallback(
    async (email: string, password: string, registerRole: Role) => {
      const result = await medflowApi.register(apiBaseUrl, {
        email,
        password,
        role: registerRole,
      });
      return persistAuth(result.accessToken, result.expiresAtUtc);
    },
    [apiBaseUrl, persistAuth],
  );

  const logout = useCallback(() => {
    setSession(null);
  }, []);

  const setApiBaseUrl = useCallback((url: string) => {
    setApiBaseUrlState(url);
  }, []);

  const value = useMemo(
    () => ({
      session,
      apiBaseUrl,
      role,
      setApiBaseUrl,
      login,
      register,
      logout,
    }),
    [session, apiBaseUrl, role, setApiBaseUrl, login, register, logout],
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth() {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error("useAuth deve ser usado dentro de AuthProvider.");
  }
  return context;
}

export function portalPathForRole(role: Role): string {
  return role === "Doctor" ? "/doctor" : "/patient";
}
