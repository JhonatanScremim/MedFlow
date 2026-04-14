using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace MedFlow.Infrastructure.Auth;

public sealed class JwtTokenIssuer(IOptions<JwtOptions> options) : IJwtTokenIssuer
{
    private readonly JwtOptions _options = options.Value;

    public AuthTokenResult Issue(JwtSubjectData subject)
    {
        if (string.IsNullOrWhiteSpace(_options.SecretKey))
            throw new InvalidOperationException("Jwt:SecretKey não está configurada.");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiresAt = DateTimeOffset.UtcNow.AddMinutes(_options.AccessTokenExpirationMinutes);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, subject.UserId.ToString()),
            new(JwtRegisteredClaimNames.Email, subject.Email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        foreach (var role in subject.RoleNames)
            claims.Add(new Claim(ClaimTypes.Role, role));

        if (subject.RoleNames.Count > 0)
            claims.Add(new Claim("roles", string.Join(',', subject.RoleNames)));

        if (subject.DoctorId is not null)
            claims.Add(new Claim("doctorId", subject.DoctorId.Value.ToString()));

        if (subject.PatientId is not null)
            claims.Add(new Claim("patientId", subject.PatientId.Value.ToString()));

        var token = new JwtSecurityToken(
            _options.Issuer,
            _options.Audience,
            claims,
            expires: expiresAt.UtcDateTime,
            signingCredentials: credentials);

        var serialized = new JwtSecurityTokenHandler().WriteToken(token);
        return new AuthTokenResult(serialized, expiresAt);
    }
}
