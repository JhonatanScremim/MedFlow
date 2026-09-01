import { FormEvent, useState } from "react";
import { Navigate, useNavigate, useSearchParams } from "react-router-dom";
import { Feedback } from "../components/Feedback";
import { portalPathForRole, useAuth } from "../auth/AuthContext";
import type { Role } from "../types/medflow";

export function LoginPage() {
  const { login, register, apiBaseUrl, setApiBaseUrl, session, role } = useAuth();
  const navigate = useNavigate();
  const [searchParams] = useSearchParams();
  const [email, setEmail] = useState(
    import.meta.env.DEV ? "patient@medflow.test" : "",
  );
  const [password, setPassword] = useState(
    import.meta.env.DEV ? "Password123!" : "",
  );
  const [registerRole, setRegisterRole] = useState<Role>("Patient");
  const [feedback, setFeedback] = useState("");
  const [loading, setLoading] = useState(false);
  const [authMode, setAuthMode] = useState<"login" | "register">("login");

  const sessionExpired = searchParams.get("reason") === "expired";

  if (session && role) {
    return <Navigate to={portalPathForRole(role)} replace />;
  }

  const handleAuth = async (mode: "login" | "register") => {
    try {
      setLoading(true);
      setAuthMode(mode);
      setFeedback("");
      const nextRole =
        mode === "login"
          ? await login(email, password)
          : await register(email, password, registerRole);
      navigate(portalPathForRole(nextRole), { replace: true });
    } catch (error) {
      setFeedback(error instanceof Error ? error.message : String(error));
    } finally {
      setLoading(false);
    }
  };

  const handleSubmit = (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    void handleAuth("login");
  };

  return (
    <main className="login-shell">
      <section className="login-card card">
        <p className="eyebrow">MedFlow</p>
        <h1>Entrar no portal</h1>
        <p className="hint">
          Faça login como paciente ou médico. Você será redirecionado para o
          portal correto.
        </p>

        {sessionExpired && (
          <Feedback message="Sessão expirada. Faça login novamente." variant="error" />
        )}

        {import.meta.env.PROD && (
          <>
            <label htmlFor="apiBaseUrl">URL da API</label>
            <input
              id="apiBaseUrl"
              value={apiBaseUrl}
              placeholder="https://sua-api.exemplo.com"
              onChange={(event) => setApiBaseUrl(event.target.value)}
            />
          </>
        )}
        {import.meta.env.DEV && (
          <p className="hint">
            Desenvolvimento: requisições usam proxy local (/api → porta 5113).
            Mantenha a API rodando com dotnet watch run.
          </p>
        )}

        <form onSubmit={handleSubmit}>
          <label htmlFor="email">E-mail</label>
          <input
            id="email"
            type="email"
            autoComplete="email"
            value={email}
            onChange={(event) => setEmail(event.target.value)}
          />

          <label htmlFor="password">Senha</label>
          <input
            id="password"
            type="password"
            autoComplete="current-password"
            value={password}
            onChange={(event) => setPassword(event.target.value)}
          />

          <label htmlFor="registerRole">Perfil para registro</label>
          <select
            id="registerRole"
            value={registerRole}
            onChange={(event) => setRegisterRole(event.target.value as Role)}
          >
            <option value="Patient">Paciente</option>
            <option value="Doctor">Médico</option>
          </select>

          {feedback && <Feedback message={feedback} variant="error" />}

          <div className="actions">
            <button type="submit" disabled={loading}>
              {loading && authMode === "login" ? "Entrando..." : "Entrar"}
            </button>
            <button
              type="button"
              className="secondary"
              disabled={loading}
              onClick={() => void handleAuth("register")}
            >
              {loading && authMode === "register"
                ? "Criando conta..."
                : "Registrar"}
            </button>
          </div>
        </form>
      </section>
    </main>
  );
}
