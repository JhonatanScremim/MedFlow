namespace MedFlow.Infrastructure.Auth;

public sealed record AuthTokenResult(string AccessToken, DateTimeOffset ExpiresAtUtc);
