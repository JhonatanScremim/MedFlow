using MedFlow.Application.Contracts.Conversation;
using MedFlow.Application.Contracts.Exam;

namespace MedFlow.Application.Realtime;

public interface INotificationService
{
    Task NotifyExamCreatedAsync(ExamResponse exam, CancellationToken cancellationToken = default);

    Task NotifyExamUpdatedAsync(ExamResponse exam, CancellationToken cancellationToken = default);

    Task NotifyMessageSentAsync(MessageResponse message, CancellationToken cancellationToken = default);
}
