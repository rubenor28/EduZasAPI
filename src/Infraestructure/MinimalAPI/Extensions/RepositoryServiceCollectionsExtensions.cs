using Application.DAOs;
using Application.DTOs.Classes;
using Application.DTOs.ClassProfessors;
using Application.DTOs.ClassStudents;
using Application.DTOs.Contacts;
using Application.DTOs.ContactTags;
using Application.DTOs.Notifications;
using Application.DTOs.Tags;
using Application.DTOs.Tests;
using Application.DTOs.UserNotifications;
using Application.DTOs.Users;
using Domain.Entities;
using EntityFramework.Application.DAOs.Classes;
using EntityFramework.Application.DAOs.ClassProfessors;
using EntityFramework.Application.DAOs.ClassStudents;
using EntityFramework.Application.DAOs.Contacts;
using EntityFramework.Application.DAOs.ContactTags;
using EntityFramework.Application.DAOs.Notifications;
using EntityFramework.Application.DAOs.Tags;
using EntityFramework.Application.DAOs.Tests;
using EntityFramework.Application.DAOs.UserNotifications;
using EntityFramework.Application.DAOs.Users;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace MinimalAPI.Extensions;

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
    public static IServiceCollection AddRepositories(
        this IServiceCollection services,
        IConfiguration cfg
    )
    {
        var pageSize = cfg.GetValue<int>("ServerOptions:PageSize");

        // USERS
        services.AddScoped<ICreatorAsync<UserDomain, NewUserDTO>, UserEFCreator>();
        services.AddScoped<IUpdaterAsync<UserDomain, UserUpdateDTO>, UserEFUpdater>();
        services.AddScoped<IReaderAsync<ulong, UserDomain>, UserEFReader>();
        services.AddScoped<IDeleterAsync<ulong, UserDomain>, UserEFDeleter>();
        services.AddScoped<IQuerierAsync<UserDomain, UserCriteriaDTO>>(
            s => new UserEFQuerier(
                s.GetRequiredService<EduZasDotnetContext>(),
                s.GetRequiredService<IMapper<User, UserDomain>>(),
                pageSize
            )
        );

        // CLASSES
        services.AddScoped<ICreatorAsync<ClassDomain, NewClassDTO>, ClassEFCreator>();
        services.AddScoped<IUpdaterAsync<ClassDomain, ClassUpdateDTO>, ClassEFUpdater>();
        services.AddScoped<IReaderAsync<string, ClassDomain>, ClassEFReader>();
        services.AddScoped<IDeleterAsync<string, ClassDomain>, ClassEFDeleter>();
        services.AddScoped<IQuerierAsync<ClassDomain, ClassCriteriaDTO>>(
            s => new ClassEFQuerier(
                s.GetRequiredService<EduZasDotnetContext>(),
                s.GetRequiredService<IMapper<Class, ClassDomain>>(),
                pageSize
             )
        );

        // CLASS PROFESSORS
        services.AddScoped<ICreatorAsync<ClassProfessorDomain, NewClassProfessorDTO>, ClassProfessorsEFCreator>();
        services.AddScoped<IUpdaterAsync<ClassProfessorDomain, ClassProfessorUpdateDTO>, ClassProfessorsEFUpdater>();
        services.AddScoped<IReaderAsync<UserClassRelationId, ClassProfessorDomain>, ClassProfessorsEFReader>();
        services.AddScoped<IDeleterAsync<UserClassRelationId, ClassProfessorDomain>, ClassProfessorsEFDeleter>();

        // CLASS STUDENTS
        services.AddScoped<ICreatorAsync<ClassStudentDomain, NewClassStudentDTO>, ClassStudentEFCreator>();
        services.AddScoped<IReaderAsync<UserClassRelationId, ClassStudentDomain>, ClassStudentsEFReader>();
        services.AddScoped<IUpdaterAsync<ClassStudentDomain, ClassStudentUpdateDTO>, ClassStudentsEFUpdater>();
        services.AddScoped<IDeleterAsync<UserClassRelationId, ClassStudentDomain>, ClassStudentsEFDeleter>();

        // NOTIFICATIONS
        services.AddScoped<IReaderAsync<ulong, NotificationDomain>, NotificationEFReader>();
        services.AddScoped<ICreatorAsync<NotificationDomain, NewNotificationDTO>, NotificationEFCreator>();
        services.AddScoped<IQuerierAsync<NotificationDomain, NotificationCriteriaDTO>>(
            s => new NotificationEFQuerier(
                s.GetRequiredService<EduZasDotnetContext>(),
                s.GetRequiredService<IMapper<Notification, NotificationDomain>>(),
                pageSize
            )
        );

        // USER NOTIFICATIONS
        services.AddScoped<ICreatorAsync<UserNotificationDomain, NewUserNotificationDTO>, UserNotificationEFCreator>();
        services.AddScoped<IUpdaterAsync<UserNotificationDomain, UserNotificationUpdateDTO>, UserNotificationEFUpdater>();
        services.AddScoped<IReaderAsync<UserNotificationIdDTO, UserNotificationDomain>, UserNotificationEFReader>();
        services.AddScoped<IDeleterAsync<UserNotificationIdDTO, UserNotificationDomain>, UserNotificationEFDelter>();

        // TAGS
        services.AddScoped<ICreatorAsync<TagDomain, NewTagDTO>, TagEFCreator>();
        services.AddScoped<IDeleterAsync<string, TagDomain>, TagEFDeleter>();
        services.AddScoped<IReaderAsync<string, TagDomain>, TagEFReader>();
        services.AddScoped<IQuerierAsync<TagDomain, TagCriteriaDTO>>(
            s => new TagEFQuerier(
                s.GetRequiredService<EduZasDotnetContext>(),
                s.GetRequiredService<IMapper<Tag, TagDomain>>(),
                pageSize
            )
        );

        // CONTACS
        services.AddScoped<IUpdaterAsync<ContactDomain, ContactUpdateDTO>, ContactEFUpdater>();
        services.AddScoped<ICreatorAsync<ContactDomain, NewContactDTO>, ContactEFCreator>();
        services.AddScoped<IDeleterAsync<ContactIdDTO, ContactDomain>, ContactEFDeleter>();
        services.AddScoped<IReaderAsync<ContactIdDTO, ContactDomain>, ContactEFReader>();
        services.AddScoped<IQuerierAsync<ContactDomain, ContactCriteriaDTO>>(
            s => new ContactEFQuerier(
                s.GetRequiredService<EduZasDotnetContext>(),
                s.GetRequiredService<IMapper<AgendaContact, ContactDomain>>(),
                pageSize
            )
        );

        // CONTACT TAGS
        services.AddScoped<ICreatorAsync<ContactTagDomain, NewContactTagDTO>, ContactTagEFCreator>();
        services.AddScoped<IDeleterAsync<ContactTagIdDTO, ContactTagDomain>, ContactTagEFDeleter>();
        services.AddScoped<IReaderAsync<ContactTagIdDTO, ContactTagDomain>, ContactTagEFReader>();

        // TEST
        services.AddScoped<ICreatorAsync<TestDomain, NewTestDTO>, TestEFCreator>();
        services.AddScoped<IUpdaterAsync<TestDomain, TestUpdateDTO>, TestEFUpdater>();
        services.AddScoped<IDeleterAsync<ulong, TestDomain>, TestEFDeleter>();
        services.AddScoped<IReaderAsync<ulong, TestDomain>, TestEFReader>();
        services.AddScoped<IQuerierAsync<TestDomain, TestCriteriaDTO>>(
            s => new TestEFQuerier(
                s.GetRequiredService<EduZasDotnetContext>(),
                s.GetRequiredService<IMapper<Test, TestDomain>>(),
                pageSize
            )
        );

        return services;
    }
}
