using Application.DTOs.Classes;
using Application.UseCases.Auth;
using Application.UseCases.Classes;
using Application.UseCases.ClassProfessors;
using Application.UseCases.ClassStudents;
using Application.UseCases.Common;
using Application.UseCases.Notifications;
using Domain.Entities;

namespace MinimalAPI.Extensions;

/// <summary>
/// Métodos de extensión para registrar los casos de uso en el contenedor de dependencias.
/// </summary>
public static class UseCaseServiceCollectionExtensions
{
    /// <summary>
    /// Registra las implementaciones de los casos de uso relacionados con <see cref="UserDomain"/>.
    /// </summary>
    /// <param name="services">La colección de servicios donde se registrarán los casos de uso.</param>
    /// <returns>La colección de servicios con los casos de uso registrados.</returns>
    public static IServiceCollection AddUseCases(this IServiceCollection services)
    {
        // Auth use cases
        services.AddTransient<AddUserUseCase>();
        services.AddTransient<LoginUseCase>();

        // Class use cases
        services.AddTransient<AddClassUseCase>();
        services.AddTransient<QueryUseCase<ClassCriteriaDTO, ClassDomain>>();
        services.AddTransient<UpdateClassUseCase>();
        services.AddTransient<DeleteClassUseCase>();

        // Class Students
        services.AddTransient<EnrollClassUseCase>();
        services.AddTransient<UnenrollClassUseCase>();
        services.AddTransient<ToggleClassVisibilityUseCase>();

        // Class professors
        services.AddTransient<AddProfessorToClassUseCase>();

        // Notifications
        services.AddTransient<AddNotificationUseCase>();
        services.AddTransient<SearchNotificationUseCase>();

        // User notifications
        services.AddTransient<MarkNotificationAsReadUseCase>();

        return services;
    }
}
