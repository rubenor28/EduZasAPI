using EduZasAPI.Application.Ports.DAOs;

using EduZasAPI.Infraestructure.Application.DTOs;
using EduZasAPI.Infraestructure.Application.Ports.DAOs;

using Microsoft.EntityFrameworkCore;

namespace EduZasAPI.Infraestructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Configuración de base de datos
        services.AddDatabaseServices(configuration);

        // Configuración de repositorios
        services.AddRepositories();

        // Configuracion de validadores
        services.AddValidators();

        // Configuracion casos de uso
        services.AddUseCases();

        // Otros servicios de infraestructura
        services.AddOtherInfrastructureServices();

        return services;
    }

    private static IServiceCollection AddDatabaseServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var conn = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<EduZasDotnetContext>(opts =>
            opts.UseMySql(conn, Microsoft.EntityFrameworkCore.ServerVersion.Parse("12.0.2-mariadb")));

        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddTransient<IUserRepositoryAsync>(provider =>
        {
            var context = provider.GetRequiredService<EduZasDotnetContext>();
            var pageSize = provider.GetRequiredService<IConfiguration>()
                .GetValue<long>("ServerOptions:PageSize");

            return new UserEntityFrameworkRepository(context, (ulong)pageSize);
        });

        return services;
    }

    private static IServiceCollection AddValidators(this IServiceCollection services)
    {
        return services;
    }

    private static IServiceCollection AddUseCases(this IServiceCollection services)
    {
        return services;
    }

    private static IServiceCollection AddOtherInfrastructureServices(this IServiceCollection services)
    {
        return services;
    }
}
