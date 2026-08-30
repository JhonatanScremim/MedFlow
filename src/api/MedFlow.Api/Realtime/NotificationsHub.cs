using MedFlow.Application.Contracts.Conversation;
using MedFlow.Application.Security;
using MedFlow.Application.UseCases.Conversation.Interfaces;
using MedFlow.Infrastructure.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace MedFlow.Api.Realtime;

[Authorize]
public sealed class NotificationsHub(
    IResourceAuthorizationService authorizationService,
    ISendMessageUseCase sendMessageUseCase) : Hub
{
    public override async Task OnConnectedAsync()
    {
        if (Context.User?.IsInRole("Doctor") == true)
            await Groups.AddToGroupAsync(Context.ConnectionId, RealtimeGroups.Doctors);

        await base.OnConnectedAsync();
    }

    public async Task JoinConversation(Guid conversationId)
    {
        if (!await CanAccessConversationAsync(conversationId))
            throw new HubException("Você não tem acesso a esta conversa.");

        await Groups.AddToGroupAsync(Context.ConnectionId, RealtimeGroups.Conversation(conversationId));
    }

    public Task LeaveConversation(Guid conversationId) =>
        Groups.RemoveFromGroupAsync(Context.ConnectionId, RealtimeGroups.Conversation(conversationId));

    public async Task<MessageResponse> SendMessage(Guid conversationId, string content)
    {
        var userId = Context.User?.GetUserId();
        if (userId is null)
            throw new HubException("Token não possui claim sub.");

        return await sendMessageUseCase.ExecuteAsync(
            conversationId,
            userId.Value,
            Context.User?.GetPatientId(),
            Context.User?.GetDoctorId(),
            new SendMessageRequest { Content = content });
    }

    private Task<bool> CanAccessConversationAsync(Guid conversationId) =>
        authorizationService.CanAccessConversationAsync(
            conversationId,
            Context.User?.GetPatientId(),
            Context.User?.GetDoctorId());
}
