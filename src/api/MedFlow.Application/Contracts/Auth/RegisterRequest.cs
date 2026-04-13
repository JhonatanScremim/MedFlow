using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using MedFlow.Infrastructure.Auth;

namespace MedFlow.Application.Contracts.Auth;

public sealed class RegisterRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(8)]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe a role: Doctor ou Patient.")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public RegistrationRole? Role { get; set; }
}
