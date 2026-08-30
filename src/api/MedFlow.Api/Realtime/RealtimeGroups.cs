namespace MedFlow.Api.Realtime;

public static class RealtimeGroups
{
    public const string Doctors = "doctors";

    public static string Conversation(Guid conversationId) => $"conversation-{conversationId:D}";
}
