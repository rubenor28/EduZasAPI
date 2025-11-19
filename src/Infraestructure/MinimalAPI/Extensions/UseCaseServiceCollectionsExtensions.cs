using Application.UseCases.Auth;
using Application.UseCases.Classes;
using Application.UseCases.ClassProfessors;
using Application.UseCases.ClassStudents;
using Application.UseCases.Contacts;
using Application.UseCases.ContactTags;
using Application.UseCases.Database;
using Application.UseCases.Notifications;
using Application.UseCases.Resources;
using Application.UseCases.Tags;
using Application.UseCases.Tests;
using Application.UseCases.UserNotifications;
using Application.UseCases.Users;
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
    /// <param name="s">La colección de servicios donde se registrarán los casos de uso.</param>
    /// <returns>La colección de servicios con los casos de uso registrados.</returns>
    public static IServiceCollection AddUseCases(this IServiceCollection s)
    {
        // Backup and restore
        s.AddScoped<BackupUseCase>();
        s.AddScoped<RestoreUseCase>();

        // Auth use cases
        s.AddTransient<AddUserUseCase>();
        s.AddTransient<LoginUseCase>();
        s.AddTransient<ReadUserUseCase>();

        // Users
        s.AddTransient<UserQueryUseCase>();
        s.AddTransient<UpdateUserUseCase>();

        // Class use cases
        s.AddTransient<AddClassUseCase>();
        s.AddTransient<QueryClassUseCase>();
        s.AddTransient<UpdateClassUseCase>();
        s.AddTransient<DeleteClassUseCase>();

        // Class Students
        s.AddTransient<AddClassStudentUseCase>();
        s.AddTransient<DeleteClassStudentUseCase>();
        s.AddTransient<UpdateClassStudentUseCase>();

        // Class professors
        s.AddTransient<AddClassProfessorUseCase>();
        s.AddTransient<UpdateClassProfessorUseCase>();
        s.AddTransient<DeleteClassProfessorUseCase>();

        // Notifications
        s.AddTransient<SearchNotificationUseCase>();

        // User notifications
        s.AddTransient<UpdateUserNotificationUseCase>();

        // Contact
        s.AddTransient<AddContactUseCase>();
        s.AddTransient<ContactQueryUseCase>();
        s.AddTransient<DeleteContactUseCase>();
        s.AddTransient<UpdateContactUseCase>();

        // Tags
        s.AddTransient<TagQueryUseCase>();

        // Contact Tag
        s.AddTransient<AddContactTagUseCase>();
        s.AddTransient<DeleteContactTagUseCase>();

        // Tests
        s.AddTransient<AddTestUseCase>();
        s.AddTransient<DeleteTestUseCase>();
        s.AddTransient<QueryTestUseCase>();
        s.AddTransient<ReadTestUseCase>();
        s.AddTransient<UpdateTestUseCase>();

        // Resource
        s.AddTransient<AddResourceUseCase>();
        s.AddTransient<ReadResourceUseCase>();
        s.AddTransient<ResourceQueryUseCase>();
        s.AddTransient<DeleteResourceUseCase>();
        s.AddTransient<UpdateResourceUseCase>();

        return s;
    }
}
