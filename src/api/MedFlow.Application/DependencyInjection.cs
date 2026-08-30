using MedFlow.Application.UseCases.Auth;
using MedFlow.Application.UseCases.Auth.Interfaces;
using MedFlow.Application.UseCases.Conversation;
using MedFlow.Application.UseCases.Conversation.Interfaces;
using MedFlow.Application.UseCases.Exam;
using MedFlow.Application.UseCases.Exam.Interfaces;
using MedFlow.Application.Security;
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
        services.AddScoped<IListExamsUseCase, ListExamsUseCase>();
        services.AddScoped<IUpdateExamStatusUseCase, UpdateExamStatusUseCase>();
        services.AddScoped<IListConversationsUseCase, ListConversationsUseCase>();
        services.AddScoped<IListConversationMessagesUseCase, ListConversationMessagesUseCase>();
        services.AddScoped<ISendMessageUseCase, SendMessageUseCase>();
        services.AddScoped<IResourceAuthorizationService, ResourceAuthorizationService>();
        return services;
    }
}
