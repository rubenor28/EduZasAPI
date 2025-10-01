using EduZasAPI.Domain.Users;
using EduZasAPI.Domain.Classes;

using EduZasAPI.Application.Common;
using EduZasAPI.Application.Users;
using EduZasAPI.Application.Classes;

using EduZasAPI.Infraestructure.EntityFramework.Application.Common;
using EduZasAPI.Infraestructure.EntityFramework.Application.Users;
using EduZasAPI.Infraestructure.EntityFramework.Application.Classes;

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
    public static IServiceCollection AddRepositories(this IServiceCollection services, IConfiguration cfg)
    {

        var pageSize = (ulong)cfg.GetValue<long>("ServerOptions:PageSize");

        // Registrar la implementación concreta una sola vez (scoped para EF DbContext)
        services.AddScoped<UserEntityFrameworkRepository>(sp => new UserEntityFrameworkRepository(
              sp.GetRequiredService<EduZasDotnetContext>(), pageSize));

        // Mapear las interfaces hacia la misma instancia concreta
        services.AddScoped<ICreatorAsync<UserDomain, NewUserDTO>>(sp => sp.GetRequiredService<UserEntityFrameworkRepository>());
        services.AddScoped<IUpdaterAsync<UserDomain, UserUpdateDTO>>(sp => sp.GetRequiredService<UserEntityFrameworkRepository>());
        services.AddScoped<IReaderAsync<ulong, UserDomain>>(sp => sp.GetRequiredService<UserEntityFrameworkRepository>());
        services.AddScoped<IDeleterAsync<ulong, UserDomain>>(sp => sp.GetRequiredService<UserEntityFrameworkRepository>());
        services.AddScoped<IQuerierAsync<UserDomain, UserCriteriaDTO>>(sp => sp.GetRequiredService<UserEntityFrameworkRepository>());

        services.AddScoped<ClassEntityFrameworkRepository>(sp => new ClassEntityFrameworkRepository(
          sp.GetRequiredService<EduZasDotnetContext>(), pageSize));

        services.AddScoped<ICreatorAsync<ClassDomain, NewClassDTO>>(sp => sp.GetRequiredService<ClassEntityFrameworkRepository>());
        services.AddScoped<IUpdaterAsync<ClassDomain, ClassUpdateDTO>>(sp => sp.GetRequiredService<ClassEntityFrameworkRepository>());
        services.AddScoped<IReaderAsync<string, ClassDomain>>(sp => sp.GetRequiredService<ClassEntityFrameworkRepository>());
        services.AddScoped<IDeleterAsync<string, ClassDomain>>(sp => sp.GetRequiredService<ClassEntityFrameworkRepository>());
        services.AddScoped<IQuerierAsync<ClassDomain, ClassCriteriaDTO>>(sp => sp.GetRequiredService<ClassEntityFrameworkRepository>());

        return services;
    }
}
