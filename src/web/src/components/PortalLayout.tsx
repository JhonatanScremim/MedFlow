import type { ConnectionStatus } from "../types/medflow";

interface PortalLayoutProps {
  title: string;
  subtitle: string;
  email?: string;
  hubStatus: ConnectionStatus;
  onLogout: () => void;
  children: React.ReactNode;
}

export function PortalLayout({
  title,
  subtitle,
  email,
  hubStatus,
  onLogout,
  children,
}: PortalLayoutProps) {
  return (
    <div className="portal-shell">
      <header className="portal-header">
        <div>
          <p className="eyebrow">MedFlow</p>
          <h1>{title}</h1>
          <p className="portal-subtitle">{subtitle}</p>
        </div>
        <div className="portal-header-actions">
          <span className={`status-pill status-${hubStatus}`}>{hubStatus}</span>
          {email && <span className="portal-user">{email}</span>}
          <button type="button" className="secondary" onClick={onLogout}>
            Sair
          </button>
        </div>
      </header>
      {children}
    </div>
  );
}
