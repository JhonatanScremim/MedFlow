using MedFlow.Infrastructure.Auth;
using MedFlow.Infrastructure.Persistence;
using MedFlow.Infrastructure.Persistence.DbContexts;
using MedFlow.Infrastructure.Persistence.Interfaces;
using MedFlow.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MedFlow.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<MedFlowDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("Database"),
                sql => sql.MigrationsAssembly(typeof(MedFlowDbContext).Assembly.GetName().Name)));

        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));

        services.AddSingleton<IJwtTokenIssuer, JwtTokenIssuer>();
        services.AddSingleton<IUserPasswordHasher, UserPasswordHasher>();
        services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();

        return services;
    }
}
