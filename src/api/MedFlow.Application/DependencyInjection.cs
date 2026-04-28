using MedFlow.Application.UseCases.Auth;
using MedFlow.Application.UseCases.Auth.Interfaces;
using MedFlow.Application.UseCases.Exam;
using MedFlow.Application.UseCases.Exam.Interfaces;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;

namespace MedFlow.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton<IMapper>(_ =>
        {
            var config = new MapperConfiguration(
                cfg => cfg.AddMaps(typeof(DependencyInjection).Assembly),
                NullLoggerFactory.Instance);
            return config.CreateMapper();
        });

        services.AddScoped<ILoginUseCase, LoginUseCase>();
        services.AddScoped<IRegisterUseCase, RegisterUseCase>();
        services.AddScoped<ICreateExamUseCase, CreateExamUseCase>();
        return services;
    }
}
