using System.ComponentModel.DataAnnotations;

namespace MedFlow.Application.Contracts.Conversation;

public sealed class SendMessageRequest
{
    [Required]
    [MaxLength(8000)]
    public string Content { get; set; } = string.Empty;
}
