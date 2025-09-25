using EduZasAPI.Domain.Users;

using EduZasAPI.Application.Users;
using EduZasAPI.Application.Common;

using EduZasAPI.Infraestructure.EntityFramework.Application.Common;
using EduZasAPI.Infraestructure.EntityFramework.Application.Users;

namespace EduZasAPI.Infraestructure.MinimalAPI.Presentation.Common;

/// <summary>
/// Métodos de extensión para registrar los repositorios en el contenedor de dependencias.
/// </summary>
public static class RepositoryServiceCollectionExtensions
{
    /// <summary>
    /// Registra las implementaciones de repositorios en el contenedor de servicios.
    /// </summary>
    /// <param name="services">La colección de servicios donde se registrarán los repositorios.</param>
    /// <returns>La colección de servicios con los repositorios registrados.</returns>
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        // Registrar la implementación concreta una sola vez (scoped para EF DbContext)
        services.AddScoped<UserEntityFrameworkRepository>(provider =>
        {
            var context = provider.GetRequiredService<EduZasDotnetContext>();
            var pageSize = provider.GetRequiredService<IConfiguration>()
                .GetValue<long>("ServerOptions:PageSize");
            return new UserEntityFrameworkRepository(context, (ulong)pageSize);
        });

        // Mapear las interfaces hacia la misma instancia concreta
        services.AddScoped<ICreatorAsync<UserDomain, NewUserDTO>>(sp => sp.GetRequiredService<UserEntityFrameworkRepository>());
        services.AddScoped<IUpdaterAsync<UserDomain, UserUpdateDTO>>(sp => sp.GetRequiredService<UserEntityFrameworkRepository>());
        services.AddScoped<IReaderAsync<ulong, UserDomain>>(sp => sp.GetRequiredService<UserEntityFrameworkRepository>());
        services.AddScoped<IDeleterAsync<ulong, UserDomain>>(sp => sp.GetRequiredService<UserEntityFrameworkRepository>());
        services.AddScoped<IQuerierAsync<UserDomain, UserCriteriaDTO>>(sp => sp.GetRequiredService<UserEntityFrameworkRepository>());

        return services;
    }
}
