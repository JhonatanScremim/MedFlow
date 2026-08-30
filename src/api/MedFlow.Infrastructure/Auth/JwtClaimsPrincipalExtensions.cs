using System.Security.Claims;

namespace MedFlow.Infrastructure.Auth;

public static class JwtClaimsPrincipalExtensions
{
    public static Guid? GetUserId(this ClaimsPrincipal principal) =>
        principal.GetGuidClaim(ClaimTypes.NameIdentifier) ??
        principal.GetGuidClaim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub) ??
        principal.GetGuidClaim("sub");

    public static Guid? GetPatientId(this ClaimsPrincipal principal) =>
        principal.GetGuidClaim("patientId");

    public static Guid? GetDoctorId(this ClaimsPrincipal principal) =>
        principal.GetGuidClaim("doctorId");

    private static Guid? GetGuidClaim(this ClaimsPrincipal principal, string claimType)
    {
        var rawValue = principal.FindFirstValue(claimType);
        return Guid.TryParse(rawValue, out var value) ? value : null;
    }
}
