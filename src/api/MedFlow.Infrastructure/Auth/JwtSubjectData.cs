namespace MedFlow.Infrastructure.Auth;

public sealed record JwtSubjectData(Guid UserId, string Email, IReadOnlyList<string> RoleNames);
