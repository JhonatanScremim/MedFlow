namespace MedFlow.Infrastructure.Auth;

public sealed record JwtSubjectData(
    Guid UserId,
    string Email,
    IReadOnlyList<string> RoleNames,
    Guid? DoctorId = null,
    Guid? PatientId = null);
