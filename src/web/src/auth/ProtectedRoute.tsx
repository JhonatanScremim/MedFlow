import { Navigate } from "react-router-dom";
import { portalPathForRole, useAuth } from "./AuthContext";
import type { Role } from "../types/medflow";

interface ProtectedRouteProps {
  role: Role;
  children: React.ReactNode;
}

export function ProtectedRoute({ role, children }: ProtectedRouteProps) {
  const { session, role: currentRole } = useAuth();

  if (!session || !currentRole) {
    return <Navigate to="/login" replace />;
  }

  if (currentRole !== role) {
    return <Navigate to={portalPathForRole(currentRole)} replace />;
  }

  return children;
}
