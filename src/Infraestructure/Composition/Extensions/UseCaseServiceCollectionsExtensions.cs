using Application.UseCases.Answers;
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
using Application.UseCases.Reports;
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
        s.AddScoped<AddUserUseCase>();
        s.AddScoped<LoginUseCase>();
        s.AddScoped<ReadUserUseCase>();
        s.AddScoped<ReadUserEmailUseCase>();

        // Users
        s.AddScoped<AddFirstAdminUserUseCase>();
        s.AddScoped<UserQueryUseCase>();
        s.AddScoped<UpdateUserUseCase>();
        s.AddScoped<DeleteUserUseCase>();

        // Class use cases
        s.AddScoped<AddClassUseCase>();
        s.AddScoped<QueryClassUseCase>();
        s.AddScoped<UpdateClassUseCase>();
        s.AddScoped<DeleteClassUseCase>();
        s.AddScoped<QueryProfessorClassesSummaryUseCase>();
        s.AddScoped<QueryStudentClassesSummaryUseCase>();

        // Class Students
        s.AddScoped<AddClassStudentUseCase>();
        s.AddScoped<DeleteClassStudentUseCase>();
        s.AddScoped<UpdateClassStudentUseCase>();
        s.AddScoped<ReadClassStudentUseCase>();

        // Class professors
        s.AddScoped<AddClassProfessorUseCase>();
        s.AddScoped<ReadClassProfessorUseCase>();
        s.AddScoped<UpdateClassProfessorUseCase>();
        s.AddScoped<DeleteClassProfessorUseCase>();
        s.AddScoped<QueryProfessorClassesSummaryUseCase>();
        s.AddScoped<QueryClassProfessorSummaryUseCase>();

        // Notifications
        s.AddScoped<SearchNotificationUseCase>();
        s.AddScoped<QueryNotificationSummaryUseCase>();
        s.AddScoped<HasUnreadNotificationUseCase>();
        s.AddScoped<UpdateUserNotificationUseCase>();

        // User notifications
        s.AddScoped<UpdateUserNotificationUseCase>();

        // Contact
        s.AddScoped<AddContactUseCase>();
        s.AddScoped<ContactQueryUseCase>();
        s.AddScoped<DeleteContactUseCase>();
        s.AddScoped<UpdateContactUseCase>();

        // Tags
        s.AddScoped<TagQueryUseCase>();

        // Contact Tag
        s.AddScoped<AddContactTagUseCase>();
        s.AddScoped<DeleteContactTagUseCase>();

        // Tests
        s.AddScoped<AddTestUseCase>();
        s.AddScoped<DeleteTestUseCase>();
        s.AddScoped<QueryTestUseCase>();
        s.AddScoped<QueryTestSummaryUseCase>();
        s.AddScoped<ReadTestUseCase>();
        s.AddScoped<ReadPublicTestUseCase>();
        s.AddScoped<UpdateTestUseCase>();
        s.AddScoped<QueryClassTestAssociationUseCase>();

        // Class Tests
        s.AddScoped<AddClassTestUseCase>();
        s.AddScoped<DeleteClassTestUseCase>();

        // Resource
        s.AddScoped<AddResourceUseCase>();
        s.AddScoped<ReadResourceUseCase>();
        s.AddScoped<ResourceQueryUseCase>();
        s.AddScoped<DeleteResourceUseCase>();
        s.AddScoped<UpdateResourceUseCase>();
        s.AddScoped<PublicReadResourceUseCase>();

        // Class resource
        s.AddScoped<AddClassResourceUseCase>();
        s.AddScoped<DeleteClassResourceUseCase>();
        s.AddScoped<ReadClassResourceUseCase>();
        s.AddScoped<UpdateClassResourceUseCase>();
        s.AddScoped<ClassResourceAssociationQueryUseCase>();

        // Class content
        s.AddScoped<QueryClassContentUseCase>();

        // Answers
        s.AddScoped<AddAnswerUseCase>();
        s.AddScoped<ReadAnswerUseCase>();
        s.AddScoped<UpdateStudentAnswerUseCase>();
        s.AddScoped<FinishTryUseCase>();
        s.AddScoped<GetAnswerStateUseCase>();

        // Reports
        s.AddScoped<AnswerGradeUseCase>();
        s.AddScoped<ClassTestAnswersGradeUseCase>();
        s.AddScoped<GlobalClassGradeUseCase>();

        return s;
    }
}
