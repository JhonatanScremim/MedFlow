import { useState } from "react";
import { Navigate, useNavigate } from "react-router-dom";
import { portalPathForRole, useAuth } from "../auth/AuthContext";
import type { Role } from "../types/medflow";

export function LoginPage() {
  const { login, register, apiBaseUrl, setApiBaseUrl, session, role } = useAuth();
  const navigate = useNavigate();
  const [email, setEmail] = useState("patient@medflow.test");
  const [password, setPassword] = useState("Password123!");
  const [registerRole, setRegisterRole] = useState<Role>("Patient");
  const [feedback, setFeedback] = useState("");
  const [loading, setLoading] = useState(false);

  if (session && role) {
    return <Navigate to={portalPathForRole(role)} replace />;
  }

  const handleAuth = async (mode: "login" | "register") => {
    try {
      setLoading(true);
      setFeedback("");
      const role =
        mode === "login"
          ? await login(email, password)
          : await register(email, password, registerRole);
      navigate(portalPathForRole(role), { replace: true });
    } catch (error) {
      setFeedback(error instanceof Error ? error.message : String(error));
    } finally {
      setLoading(false);
    }
  };

  return (
    <main className="login-shell">
      <section className="login-card card">
        <p className="eyebrow">MedFlow</p>
        <h1>Entrar no portal</h1>
        <p className="hint">
          Faca login como paciente ou medico. Voce sera redirecionado para o
          portal correto.
        </p>

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
            Desenvolvimento: requisicoes usam proxy local (/api → porta 5113).
            Mantenha a API rodando com dotnet watch run.
          </p>
        )}

        <label htmlFor="email">Email</label>
        <input
          id="email"
          type="email"
          value={email}
          onChange={(event) => setEmail(event.target.value)}
        />

        <label htmlFor="password">Senha</label>
        <input
          id="password"
          type="password"
          value={password}
          onChange={(event) => setPassword(event.target.value)}
        />

        <label htmlFor="registerRole">Role para registro</label>
        <select
          id="registerRole"
          value={registerRole}
          onChange={(event) => setRegisterRole(event.target.value as Role)}
        >
          <option value="Patient">Patient</option>
          <option value="Doctor">Doctor</option>
        </select>

        {feedback && <p className="feedback error">{feedback}</p>}

        <div className="actions">
          <button
            type="button"
            disabled={loading}
            onClick={() => void handleAuth("login")}
          >
            Entrar
          </button>
          <button
            type="button"
            className="secondary"
            disabled={loading}
            onClick={() => void handleAuth("register")}
          >
            Registrar
          </button>
        </div>
      </section>
    </main>
  );
}
