using MedFlow.Application.Contracts.Auth;
using MedFlow.Infrastructure.Auth;

namespace MedFlow.Application.UseCases.Auth.Interfaces;

public interface IRegisterUseCase
{
    Task<AuthTokenResult> ExecuteAsync(RegisterRequest request);
}
