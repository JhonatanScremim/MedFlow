using MedFlow.Application.Contracts.Auth;
using MedFlow.Application.Exceptions;
using MedFlow.Application.UseCases.Auth.Interfaces;
using MedFlow.Infrastructure.Auth;
using MedFlow.Infrastructure.Persistence.Interfaces;
using MedFlow.Infrastructure.Security;

namespace MedFlow.Application.UseCases.Auth;

public sealed class LoginUseCase(
    IUserRepository userRepository,
    IUserPasswordHasher passwordHasher,
    IJwtTokenIssuer jwtTokenIssuer) : ILoginUseCase
{
    public async Task<AuthTokenResult> ExecuteAsync(LoginRequest request)
    {
        var (normalizedEmail, pwd) = AuthValidation.ValidateAndNormalize(request.Email, request.Password);

        var user = await userRepository.GetByNormalizedEmailWithRolesAsync(normalizedEmail);
        if (user is null || !passwordHasher.Verify(user.PasswordHash, pwd))
            throw new AuthenticationException("Credenciais inválidas");

        var roles = user.UserRoles.Select(ur => ur.Role.Name).ToArray();
        return jwtTokenIssuer.Issue(new JwtSubjectData(
            user.Id,
            user.Email,
            roles,
            user.Doctor?.Id,
            user.Patient?.Id));
    }
}
