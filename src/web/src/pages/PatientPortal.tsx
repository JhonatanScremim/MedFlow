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
  type ExamType,
  type MessageResponse,
} from "../types/medflow";
import { upsertById } from "../utils/upsert";

export function PatientPortal() {
  const { session, apiBaseUrl, logout } = useAuth();
  const navigate = useNavigate();
  const [exams, setExams] = useState<ExamResponse[]>([]);
  const [feedback, setFeedback] = useState("");
  const [chatMessages, setChatMessages] = useState<MessageResponse[]>([]);
  const [chatRefreshKey, setChatRefreshKey] = useState(0);
  const [createExamForm, setCreateExamForm] = useState({
    type: "1",
    doctorId: "",
    scheduledAtUtc: "",
    notes: "Exame solicitado pelo portal do paciente.",
  });

  const token = session!.token;

  const hub = useMedFlowHub({
    apiBaseUrl,
    accessToken: token,
    onExamCreated: useCallback(() => {}, []),
    onExamUpdated: useCallback((exam) => {
      setExams((current) => upsertById(current, exam));
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

  const createExam = async () => {
    try {
      const created = await medflowApi.createExam(apiBaseUrl, token, {
        type: Number(createExamForm.type) as ExamType,
        doctorId: createExamForm.doctorId || undefined,
        scheduledAtUtc: createExamForm.scheduledAtUtc
          ? new Date(createExamForm.scheduledAtUtc).toISOString()
          : undefined,
        notes: createExamForm.notes || undefined,
      });
      setExams((current) => upsertById(current, created));
      setChatRefreshKey((current) => current + 1);
      setFeedback(`Exame criado: ${created.id}`);
    } catch (error) {
      setFeedback(error instanceof Error ? error.message : String(error));
    }
  };

  return (
    <PortalLayout
      title="Portal do Paciente"
      subtitle="Solicite exames e converse com o medico responsavel."
      email={session?.profile.email}
      hubStatus={hub.status}
      onLogout={() => void handleLogout()}
    >
      <div className="portal-grid">
        <section className="card portal-main">
          <div className="section-title">
            <h2>Meus exames</h2>
            <button type="button" className="secondary" onClick={() => void loadExams()}>
              Atualizar
            </button>
          </div>

          {feedback && <p className="feedback">{feedback}</p>}

          <label htmlFor="examType">Tipo de exame</label>
          <select
            id="examType"
            value={createExamForm.type}
            onChange={(event) =>
              setCreateExamForm((current) => ({
                ...current,
                type: event.target.value,
              }))
            }
          >
            {Object.entries(examTypeLabels).map(([value, label]) => (
              <option key={value} value={value}>
                {value} - {label}
              </option>
            ))}
          </select>

          <label htmlFor="doctorId">DoctorId opcional</label>
          <input
            id="doctorId"
            value={createExamForm.doctorId}
            onChange={(event) =>
              setCreateExamForm((current) => ({
                ...current,
                doctorId: event.target.value,
              }))
            }
            placeholder="Deixe vazio para fila aberta"
          />

          <label htmlFor="notes">Notas</label>
          <textarea
            id="notes"
            value={createExamForm.notes}
            onChange={(event) =>
              setCreateExamForm((current) => ({
                ...current,
                notes: event.target.value,
              }))
            }
          />

          <button type="button" onClick={() => void createExam()}>
            Solicitar exame
          </button>

          <div className="list exam-list">
            {exams.map((exam) => (
              <div className="resource-card" key={exam.id}>
                <strong>{examTypeLabels[exam.type]}</strong>
                <span>Status: {examStatusLabels[exam.status]}</span>
                <span>Medico: {exam.doctorId ?? "fila aberta"}</span>
                <span>Conversa: {exam.conversationId ?? "pendente"}</span>
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
