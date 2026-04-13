namespace MedFlow.Infrastructure.Auth;

public interface IJwtTokenIssuer
{
    AuthTokenResult Issue(JwtSubjectData subject);
}
