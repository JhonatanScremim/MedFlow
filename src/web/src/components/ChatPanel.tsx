import { useCallback, useEffect, useState } from "react";
import { medflowApi } from "../api/medflowApi";
import type { ConversationResponse, MessageResponse } from "../types/medflow";

export interface HubActions {
  joinConversation: (conversationId: string) => Promise<void>;
  leaveConversation: (conversationId: string) => Promise<void>;
  sendHubMessage: (conversationId: string, content: string) => Promise<void>;
}

interface ChatPanelProps {
  apiBaseUrl: string;
  accessToken: string;
  hub: HubActions;
  messages: MessageResponse[];
  setMessages: React.Dispatch<React.SetStateAction<MessageResponse[]>>;
  refreshKey?: number;
}

export function ChatPanel({
  apiBaseUrl,
  accessToken,
  hub,
  messages,
  setMessages,
  refreshKey = 0,
}: ChatPanelProps) {
  const [conversations, setConversations] = useState<ConversationResponse[]>([]);
  const [selectedConversationId, setSelectedConversationId] = useState("");
  const [draft, setDraft] = useState("");
  const [feedback, setFeedback] = useState("");
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    let active = true;

    async function fetchConversations() {
      try {
        setLoading(true);
        const next = await medflowApi.listConversations(apiBaseUrl, accessToken);
        if (!active) {
          return;
        }
        setConversations(next);
        setSelectedConversationId((current) => current || next[0]?.id || "");
      } catch (error) {
        if (active) {
          setFeedback(error instanceof Error ? error.message : String(error));
        }
      } finally {
        if (active) {
          setLoading(false);
        }
      }
    }

    void fetchConversations();

    return () => {
      active = false;
    };
  }, [apiBaseUrl, accessToken, refreshKey]);

  useEffect(() => {
    if (!selectedConversationId) {
      return;
    }

    let active = true;

    async function fetchMessages() {
      try {
        setLoading(true);
        const next = await medflowApi.listMessages(
          apiBaseUrl,
          accessToken,
          selectedConversationId,
        );
        if (!active) {
          return;
        }
        setMessages(next.slice().reverse());
        await hub.joinConversation(selectedConversationId);
      } catch (error) {
        if (active) {
          setFeedback(error instanceof Error ? error.message : String(error));
        }
      } finally {
        if (active) {
          setLoading(false);
        }
      }
    }

    void fetchMessages();

    return () => {
      active = false;
    };
  }, [apiBaseUrl, accessToken, selectedConversationId, hub, setMessages]);

  const refreshConversations = useCallback(async () => {
    try {
      setLoading(true);
      const next = await medflowApi.listConversations(apiBaseUrl, accessToken);
      setConversations(next);
    } catch (error) {
      setFeedback(error instanceof Error ? error.message : String(error));
    } finally {
      setLoading(false);
    }
  }, [apiBaseUrl, accessToken]);

  const handleSelectConversation = async (conversationId: string) => {
    if (selectedConversationId && selectedConversationId !== conversationId) {
      await hub.leaveConversation(selectedConversationId);
    }
    setSelectedConversationId(conversationId);
  };

  const handleSend = async () => {
    if (!selectedConversationId || !draft.trim()) {
      return;
    }

    try {
      await hub.sendHubMessage(selectedConversationId, draft.trim());
      setDraft("");
    } catch (error) {
      setFeedback(error instanceof Error ? error.message : String(error));
    }
  };

  const selectedConversation = conversations.find(
    (conversation) => conversation.id === selectedConversationId,
  );

  return (
    <section className="chat-panel card">
      <div className="section-title">
        <h2>Chat</h2>
        <button type="button" className="secondary" onClick={() => void refreshConversations()}>
          Atualizar
        </button>
      </div>

      {feedback && <p className="feedback">{feedback}</p>}

      <div className="chat-layout">
        <aside className="chat-sidebar">
          {loading && conversations.length === 0 && (
            <p className="hint">Carregando conversas...</p>
          )}
          {!loading && conversations.length === 0 && (
            <p className="hint">Nenhuma conversa disponivel ainda.</p>
          )}
          {conversations.map((conversation) => (
            <button
              type="button"
              key={conversation.id}
              className={`list-button ${
                conversation.id === selectedConversationId ? "active" : ""
              }`}
              onClick={() => void handleSelectConversation(conversation.id)}
            >
              <strong>Exame {conversation.examId.slice(0, 8)}...</strong>
              <span>
                Medico: {conversation.doctorId ?? "nao atribuido"}
              </span>
            </button>
          ))}
        </aside>

        <div className="chat-main">
          {selectedConversation ? (
            <>
              <p className="hint">
                Conversa do exame {selectedConversation.examId}
              </p>
              <div className="list messages">
                {messages.map((message) => (
                  <div className="message" key={message.id}>
                    <strong>{message.content}</strong>
                    <span>
                      {new Date(message.sentAt).toLocaleString()} por{" "}
                      {message.senderUserId ?? "sistema"}
                    </span>
                  </div>
                ))}
              </div>
              <textarea
                value={draft}
                onChange={(event) => setDraft(event.target.value)}
                placeholder="Digite sua mensagem..."
              />
              <button type="button" onClick={() => void handleSend()}>
                Enviar mensagem
              </button>
            </>
          ) : (
            <p className="hint">Selecione uma conversa para comecar.</p>
          )}
        </div>
      </div>
    </section>
  );
}
