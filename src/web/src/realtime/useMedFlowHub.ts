import {
  HubConnection,
  HubConnectionBuilder,
  HubConnectionState,
  LogLevel,
} from "@microsoft/signalr";
import { useCallback, useEffect, useRef, useState } from "react";
import { trimTrailingSlash } from "../api/medflowApi";
import type {
  ConnectionStatus,
  ExamResponse,
  MessageResponse,
} from "../types/medflow";

interface UseMedFlowHubOptions {
  apiBaseUrl: string;
  accessToken?: string;
  onExamCreated: (exam: ExamResponse) => void;
  onExamUpdated: (exam: ExamResponse) => void;
  onMessageReceived: (message: MessageResponse) => void;
  onSystemMessage: (message: string) => void;
}

export function useMedFlowHub({
  apiBaseUrl,
  accessToken,
  onExamCreated,
  onExamUpdated,
  onMessageReceived,
  onSystemMessage,
}: UseMedFlowHubOptions) {
  const connectionRef = useRef<HubConnection | null>(null);
  const [status, setStatus] = useState<ConnectionStatus>("disconnected");

  const disconnect = useCallback(async () => {
    const connection = connectionRef.current;
    connectionRef.current = null;

    if (connection && connection.state !== HubConnectionState.Disconnected) {
      await connection.stop();
    }

    setStatus("disconnected");
  }, []);

  const connect = useCallback(async () => {
    if (!accessToken) {
      onSystemMessage("Informe um token antes de conectar no Hub.");
      return;
    }

    if (connectionRef.current?.state === HubConnectionState.Connected) {
      return;
    }

    setStatus("connecting");

    const connection = new HubConnectionBuilder()
      .withUrl(`${trimTrailingSlash(apiBaseUrl)}/hubs/notifications`, {
        accessTokenFactory: () => accessToken,
      })
      .withAutomaticReconnect()
      .configureLogging(LogLevel.Information)
      .build();

    connection.on("ExamCreated", onExamCreated);
    connection.on("ExamUpdated", onExamUpdated);
    connection.on("ReceiveMessage", onMessageReceived);

    connection.onreconnecting(() => setStatus("reconnecting"));
    connection.onreconnected(() => {
      setStatus("connected");
      onSystemMessage("Conexao SignalR restabelecida.");
    });
    connection.onclose(() => setStatus("disconnected"));

    try {
      await connection.start();
      connectionRef.current = connection;
      setStatus("connected");
      onSystemMessage("Conectado ao Hub SignalR.");
    } catch (error) {
      setStatus("disconnected");
      onSystemMessage(error instanceof Error ? error.message : String(error));
    }
  }, [
    accessToken,
    apiBaseUrl,
    onExamCreated,
    onExamUpdated,
    onMessageReceived,
    onSystemMessage,
  ]);

  const joinConversation = useCallback(
    async (conversationId: string) => {
      await connectionRef.current?.invoke("JoinConversation", conversationId);
      onSystemMessage(`Entrou na conversa ${conversationId}.`);
    },
    [onSystemMessage],
  );

  const leaveConversation = useCallback(
    async (conversationId: string) => {
      await connectionRef.current?.invoke("LeaveConversation", conversationId);
      onSystemMessage(`Saiu da conversa ${conversationId}.`);
    },
    [onSystemMessage],
  );

  const sendHubMessage = useCallback(
    async (conversationId: string, content: string) => {
      const message = await connectionRef.current?.invoke<MessageResponse>(
        "SendMessage",
        conversationId,
        content,
      );

      if (message) {
        onSystemMessage(`Mensagem enviada pelo Hub: ${message.id}.`);
      }
    },
    [onSystemMessage],
  );

  useEffect(() => () => void disconnect(), [disconnect]);

  return {
    status,
    connect,
    disconnect,
    joinConversation,
    leaveConversation,
    sendHubMessage,
  };
}
