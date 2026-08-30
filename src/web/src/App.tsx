import { Navigate, Route, Routes } from "react-router-dom";
import { ProtectedRoute } from "./auth/ProtectedRoute";
import { portalPathForRole, useAuth } from "./auth/AuthContext";
import { DoctorPortal } from "./pages/DoctorPortal";
import { LoginPage } from "./pages/LoginPage";
import { PatientPortal } from "./pages/PatientPortal";

function HomeRedirect() {
  const { session, role } = useAuth();

  if (!session || !role) {
    return <Navigate to="/login" replace />;
  }

  return <Navigate to={portalPathForRole(role)} replace />;
}

export default function App() {
  return (
    <Routes>
      <Route path="/" element={<HomeRedirect />} />
      <Route path="/login" element={<LoginPage />} />
      <Route
        path="/patient"
        element={
          <ProtectedRoute role="Patient">
            <PatientPortal />
          </ProtectedRoute>
        }
      />
      <Route
        path="/doctor"
        element={
          <ProtectedRoute role="Doctor">
            <DoctorPortal />
          </ProtectedRoute>
        }
      />
      <Route path="*" element={<Navigate to="/" replace />} />
    </Routes>
  );
}
