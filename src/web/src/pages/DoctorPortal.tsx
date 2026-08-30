import { useCallback, useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { medflowApi } from "../api/medflowApi";
import { useAuth } from "../auth/AuthContext";
import { ChatPanel } from "../components/ChatPanel";
import { PortalLayout } from "../components/PortalLayout";
import { useMedFlowHub } from "../realtime/useMedFlowHub";
import {
  examStatusLabels,
  examTypeLabels,
  type ExamResponse,
  type ExamStatus,
  type MessageResponse,
} from "../types/medflow";
import { upsertById } from "../utils/upsert";

export function DoctorPortal() {
  const { session, apiBaseUrl, logout } = useAuth();
  const navigate = useNavigate();
  const [exams, setExams] = useState<ExamResponse[]>([]);
  const [feedback, setFeedback] = useState("");
  const [chatMessages, setChatMessages] = useState<MessageResponse[]>([]);
  const [chatRefreshKey, setChatRefreshKey] = useState(0);
  const [statusForm, setStatusForm] = useState({
    examId: "",
    status: "1",
  });

  const token = session!.token;

  const hub = useMedFlowHub({
    apiBaseUrl,
    accessToken: token,
    onExamCreated: useCallback((exam) => {
      setExams((current) => upsertById(current, exam));
      setFeedback(`Novo exame recebido: ${exam.id}`);
    }, []),
    onExamUpdated: useCallback((exam) => {
      setExams((current) => upsertById(current, exam));
      setChatRefreshKey((current) => current + 1);
    }, []),
    onMessageReceived: useCallback((message) => {
      setChatMessages((current) => upsertById(current, message));
    }, []),
    onSystemMessage: useCallback((message) => {
      setFeedback(message);
    }, []),
  });

  useEffect(() => {
    void hub.connect();
    return () => {
      void hub.disconnect();
    };
    // eslint-disable-next-line react-hooks/exhaustive-deps -- connect once per portal mount
  }, [session?.token]);

  useEffect(() => {
    let active = true;

    async function fetchExams() {
      try {
        const next = await medflowApi.listExams(apiBaseUrl, token);
        if (active) {
          setExams(next);
        }
      } catch (error) {
        if (active) {
          setFeedback(error instanceof Error ? error.message : String(error));
        }
      }
    }

    void fetchExams();

    return () => {
      active = false;
    };
  }, [apiBaseUrl, token]);

  const loadExams = useCallback(async () => {
    try {
      const next = await medflowApi.listExams(apiBaseUrl, token);
      setExams(next);
    } catch (error) {
      setFeedback(error instanceof Error ? error.message : String(error));
    }
  }, [apiBaseUrl, token]);

  const handleLogout = async () => {
    await hub.disconnect();
    logout();
    navigate("/login", { replace: true });
  };

  const updateExamStatus = async () => {
    if (!statusForm.examId) {
      setFeedback("Informe o ExamId.");
      return;
    }

    try {
      const updated = await medflowApi.updateExamStatus(
        apiBaseUrl,
        token,
        statusForm.examId,
        { status: Number(statusForm.status) as ExamStatus },
      );
      setExams((current) => upsertById(current, updated));
      setChatRefreshKey((current) => current + 1);
      setFeedback(`Exame atualizado: ${updated.id}`);
    } catch (error) {
      setFeedback(error instanceof Error ? error.message : String(error));
    }
  };

  return (
    <PortalLayout
      title="Portal do Medico"
      subtitle="Acompanhe a fila de exames e converse com pacientes."
      email={session?.profile.email}
      hubStatus={hub.status}
      onLogout={() => void handleLogout()}
    >
      <div className="portal-grid">
        <section className="card portal-main">
          <div className="section-title">
            <h2>Fila de exames</h2>
            <button type="button" className="secondary" onClick={() => void loadExams()}>
              Atualizar
            </button>
          </div>

          {feedback && <p className="feedback">{feedback}</p>}

          <label htmlFor="examIdStatus">ExamId</label>
          <input
            id="examIdStatus"
            value={statusForm.examId}
            onChange={(event) =>
              setStatusForm((current) => ({
                ...current,
                examId: event.target.value,
              }))
            }
            placeholder="Selecione um exame abaixo ou cole o id"
          />

          <label htmlFor="examStatus">Status</label>
          <select
            id="examStatus"
            value={statusForm.status}
            onChange={(event) =>
              setStatusForm((current) => ({
                ...current,
                status: event.target.value,
              }))
            }
          >
            {Object.entries(examStatusLabels).map(([value, label]) => (
              <option key={value} value={value}>
                {value} - {label}
              </option>
            ))}
          </select>

          <button type="button" onClick={() => void updateExamStatus()}>
            Assumir / atualizar status
          </button>

          <div className="list exam-list">
            {exams.map((exam) => (
              <div className="resource-card" key={exam.id}>
                <strong>{examTypeLabels[exam.type]}</strong>
                <span>Status: {examStatusLabels[exam.status]}</span>
                <span>Paciente: {exam.patientId}</span>
                <span>Medico: {exam.doctorId ?? "fila aberta"}</span>
                <div className="actions">
                  <button
                    type="button"
                    className="secondary"
                    onClick={() =>
                      setStatusForm((current) => ({
                        ...current,
                        examId: exam.id,
                      }))
                    }
                  >
                    Usar este exame
                  </button>
                </div>
              </div>
            ))}
          </div>
        </section>

        <ChatPanel
          apiBaseUrl={apiBaseUrl}
          accessToken={token}
          hub={hub}
          messages={chatMessages}
          setMessages={setChatMessages}
          refreshKey={chatRefreshKey}
        />
      </div>
    </PortalLayout>
  );
}
