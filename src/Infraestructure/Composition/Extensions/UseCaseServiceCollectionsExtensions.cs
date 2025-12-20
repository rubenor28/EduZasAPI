using Application.UseCases.Auth;
using Application.UseCases.ClassContent;
using Application.UseCases.Classes;
using Application.UseCases.ClassProfessors;
using Application.UseCases.ClassResource;
using Application.UseCases.ClassStudents;
using Application.UseCases.ClassTests;
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
using Microsoft.Extensions.DependencyInjection;

namespace Composition.Extensions;

/// <summary>
/// Métodos de extensión para registrar los casos de uso en el contenedor de dependencias.
/// </summary>
internal static class UseCaseServiceCollectionExtensions
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
        s.AddTransient<ReadUserEmailUseCase>();

        // Users
        s.AddTransient<AddFirstAdminUserUseCase>();
        s.AddTransient<UserQueryUseCase>();
        s.AddTransient<UpdateUserUseCase>();
        s.AddTransient<DeleteUserUseCase>();

        // Class use cases
        s.AddTransient<AddClassUseCase>();
        s.AddTransient<QueryClassUseCase>();
        s.AddTransient<UpdateClassUseCase>();
        s.AddTransient<DeleteClassUseCase>();
        s.AddTransient<QueryProfessorClassesSummaryUseCase>();
        s.AddTransient<QueryStudentClassesSummaryUseCase>();

        // Class Students
        s.AddTransient<AddClassStudentUseCase>();
        s.AddTransient<DeleteClassStudentUseCase>();
        s.AddTransient<UpdateClassStudentUseCase>();
        s.AddTransient<ReadClassStudentUseCase>();

        // Class professors
        s.AddTransient<AddClassProfessorUseCase>();
        s.AddTransient<ReadClassProfessorUseCase>();
        s.AddTransient<UpdateClassProfessorUseCase>();
        s.AddTransient<DeleteClassProfessorUseCase>();
        s.AddTransient<QueryProfessorClassesSummaryUseCase>();
        s.AddTransient<QueryClassProfessorSummaryUseCase>();

        // Notifications
        s.AddTransient<SearchNotificationUseCase>();
        s.AddTransient<QueryNotificationSummaryUseCase>();
        s.AddTransient<HasUnreadNotificationUseCase>();
        s.AddTransient<UpdateUserNotificationUseCase>();

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
        s.AddTransient<QueryTestSummaryUseCase>();
        s.AddTransient<ReadTestUseCase>();
        s.AddTransient<ReadPublicTestUseCase>();
        s.AddTransient<UpdateTestUseCase>();
        s.AddTransient<QueryClassTestAssociationUseCase>();

        // Class Tests
        s.AddTransient<AddClassTestUseCase>();
        s.AddTransient<UpdateClassTestUseCase>();
        s.AddTransient<DeleteClassTestUseCase>();

        // Resource
        s.AddTransient<AddResourceUseCase>();
        s.AddTransient<ReadResourceUseCase>();
        s.AddTransient<ResourceQueryUseCase>();
        s.AddTransient<DeleteResourceUseCase>();
        s.AddTransient<UpdateResourceUseCase>();
        s.AddTransient<PublicReadResourceUseCase>();

        // Class resource
        s.AddTransient<AddClassResourceUseCase>();
        s.AddTransient<DeleteClassResourceUseCase>();
        s.AddTransient<ReadClassResourceUseCase>();
        s.AddTransient<UpdateClassResourceUseCase>();
        s.AddTransient<ClassResourceAssociationQueryUseCase>();

        // Class content
        s.AddTransient<QueryClassContentUseCase>();

        return s;
    }
}
