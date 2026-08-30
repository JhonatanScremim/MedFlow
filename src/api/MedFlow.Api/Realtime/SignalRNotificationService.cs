using MedFlow.Application.Contracts.Conversation;
using MedFlow.Application.Contracts.Exam;
using MedFlow.Application.Realtime;
using Microsoft.AspNetCore.SignalR;

namespace MedFlow.Api.Realtime;

public sealed class SignalRNotificationService(
    IHubContext<NotificationsHub> hubContext) : INotificationService
{
    public Task NotifyExamCreatedAsync(ExamResponse exam, CancellationToken cancellationToken = default) =>
        hubContext.Clients
            .Group(RealtimeGroups.Doctors)
            .SendAsync("ExamCreated", exam, cancellationToken);

    public async Task NotifyExamUpdatedAsync(ExamResponse exam, CancellationToken cancellationToken = default)
    {
        await hubContext.Clients
            .Group(RealtimeGroups.Doctors)
            .SendAsync("ExamUpdated", exam, cancellationToken);

        if (exam.ConversationId is not null)
        {
            await hubContext.Clients
                .Group(RealtimeGroups.Conversation(exam.ConversationId.Value))
                .SendAsync("ExamUpdated", exam, cancellationToken);
        }
    }

    public Task NotifyMessageSentAsync(MessageResponse message, CancellationToken cancellationToken = default) =>
        hubContext.Clients
            .Group(RealtimeGroups.Conversation(message.ConversationId))
            .SendAsync("ReceiveMessage", message, cancellationToken);
}
