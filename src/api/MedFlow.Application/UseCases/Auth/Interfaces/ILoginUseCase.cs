using MedFlow.Application.Contracts.Auth;
using MedFlow.Infrastructure.Auth;

namespace MedFlow.Application.UseCases.Auth.Interfaces;

public interface ILoginUseCase
{
    Task<AuthTokenResult> ExecuteAsync(LoginRequest request);
}
