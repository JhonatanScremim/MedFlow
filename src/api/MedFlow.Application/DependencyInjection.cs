using MedFlow.Application.UseCases.Auth;
using MedFlow.Application.UseCases.Auth.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace MedFlow.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ILoginUseCase, LoginUseCase>();
        services.AddScoped<IRegisterUseCase, RegisterUseCase>();
        return services;
    }
}
