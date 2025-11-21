using Application.DAOs;
using Application.DTOs.Classes;
using Application.DTOs.ClassProfessors;
using Application.DTOs.ClassStudents;
using Application.DTOs.Contacts;
using Application.DTOs.ContactTags;
using Application.DTOs.Notifications;
using Application.DTOs.Resources;
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
using EntityFramework.Application.DAOs.Resources;
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
    /// <param name="s">La colección de servicios donde se registrarán los repositorios.</param>
    /// <returns>La colección de servicios con los repositorios registrados.</returns>
    public static IServiceCollection AddRepositories(this IServiceCollection s, IConfiguration cfg)
    {
        var pageSize = cfg.GetValue<int>("ServerOptions:PageSize");

        // USERS
        s.AddScoped<ICreatorAsync<UserDomain, NewUserDTO>, UserEFCreator>();
        s.AddScoped<IUpdaterAsync<UserDomain, UserUpdateDTO>, UserEFUpdater>();
        s.AddScoped<IReaderAsync<ulong, UserDomain>, UserEFReader>();
        s.AddScoped<IReaderAsync<string, UserDomain>, UserEmailEFReader>();
        s.AddScoped<IDeleterAsync<ulong, UserDomain>, UserEFDeleter>();
        s.AddScoped<IQuerierAsync<UserDomain, UserCriteriaDTO>>(
            sp => new UserEFQuerier(
                sp.GetRequiredService<EduZasDotnetContext>(),
                sp.GetRequiredService<IMapper<User, UserDomain>>(),
                pageSize
            )
        );

        // CLASSES
        s.AddScoped<ICreatorAsync<ClassDomain, NewClassDTO>, ClassEFCreator>();
        s.AddScoped<IUpdaterAsync<ClassDomain, ClassUpdateDTO>, ClassEFUpdater>();
        s.AddScoped<IReaderAsync<string, ClassDomain>, ClassEFReader>();
        s.AddScoped<IDeleterAsync<string, ClassDomain>, ClassEFDeleter>();
        s.AddScoped<IQuerierAsync<ClassDomain, ClassCriteriaDTO>>(
            sp => new ClassEFQuerier(
                sp.GetRequiredService<EduZasDotnetContext>(),
                sp.GetRequiredService<IMapper<Class, ClassDomain>>(),
                pageSize
             )
        );

        // CLASS PROFESSORS
        s.AddScoped<ICreatorAsync<ClassProfessorDomain, NewClassProfessorDTO>, ClassProfessorsEFCreator>();
        s.AddScoped<IUpdaterAsync<ClassProfessorDomain, ClassProfessorUpdateDTO>, ClassProfessorsEFUpdater>();
        s.AddScoped<IReaderAsync<UserClassRelationId, ClassProfessorDomain>, ClassProfessorsEFReader>();
        s.AddScoped<IDeleterAsync<UserClassRelationId, ClassProfessorDomain>, ClassProfessorsEFDeleter>();

        // CLASS STUDENTS
        s.AddScoped<ICreatorAsync<ClassStudentDomain, NewClassStudentDTO>, ClassStudentEFCreator>();
        s.AddScoped<IReaderAsync<UserClassRelationId, ClassStudentDomain>, ClassStudentsEFReader>();
        s.AddScoped<IUpdaterAsync<ClassStudentDomain, ClassStudentUpdateDTO>, ClassStudentsEFUpdater>();
        s.AddScoped<IDeleterAsync<UserClassRelationId, ClassStudentDomain>, ClassStudentsEFDeleter>();

        // NOTIFICATIONS
        s.AddScoped<NotificationEFCreator>();
        s.AddScoped<ICreatorAsync<NotificationDomain, NewNotificationDTO>>(sp => sp.GetRequiredService<NotificationEFCreator>());
        s.AddScoped<IBulkCreatorAsync<NotificationDomain, NewNotificationDTO>>(sp => sp.GetRequiredService<NotificationEFCreator>());
        s.AddScoped<IReaderAsync<ulong, NotificationDomain>, NotificationEFReader>();
        s.AddScoped<IQuerierAsync<NotificationDomain, NotificationCriteriaDTO>>(
            sp => new NotificationEFQuerier(
                sp.GetRequiredService<EduZasDotnetContext>(),
                sp.GetRequiredService<IMapper<Notification, NotificationDomain>>(),
                pageSize
            )
        );

        // USER NOTIFICATIONS
        s.AddScoped<UserNotificationEFCreator>();
        s.AddScoped<ICreatorAsync<UserNotificationDomain, NewUserNotificationDTO>>(sp => sp.GetRequiredService<UserNotificationEFCreator>());
        s.AddScoped<IBulkCreatorAsync<UserNotificationDomain, NewUserNotificationDTO>>(sp => sp.GetRequiredService<UserNotificationEFCreator>());
        s.AddScoped<IUpdaterAsync<UserNotificationDomain, UserNotificationUpdateDTO>, UserNotificationEFUpdater>();
        s.AddScoped<IReaderAsync<UserNotificationIdDTO, UserNotificationDomain>, UserNotificationEFReader>();

        // TAGS
        s.AddScoped<ICreatorAsync<TagDomain, NewTagDTO>, TagEFCreator>();
        s.AddScoped<IDeleterAsync<string, TagDomain>, TagEFDeleter>();
        s.AddScoped<IReaderAsync<string, TagDomain>, TagEFReader>();
        s.AddScoped<IQuerierAsync<TagDomain, TagCriteriaDTO>>(
            sp => new TagEFQuerier(
                sp.GetRequiredService<EduZasDotnetContext>(),
                sp.GetRequiredService<IMapper<Tag, TagDomain>>(),
                pageSize
            )
        );

        // CONTACS
        s.AddScoped<IUpdaterAsync<ContactDomain, ContactUpdateDTO>, ContactEFUpdater>();
        s.AddScoped<ICreatorAsync<ContactDomain, NewContactDTO>, ContactEFCreator>();
        s.AddScoped<IDeleterAsync<ContactIdDTO, ContactDomain>, ContactEFDeleter>();
        s.AddScoped<IReaderAsync<ContactIdDTO, ContactDomain>, ContactEFReader>();
        s.AddScoped<IQuerierAsync<ContactDomain, ContactCriteriaDTO>>(
            sp => new ContactEFQuerier(
                sp.GetRequiredService<EduZasDotnetContext>(),
                sp.GetRequiredService<IMapper<AgendaContact, ContactDomain>>(),
                pageSize
            )
        );

        // CONTACT TAGS
        s.AddScoped<ICreatorAsync<ContactTagDomain, NewContactTagDTO>, ContactTagEFCreator>();
        s.AddScoped<IDeleterAsync<ContactTagIdDTO, ContactTagDomain>, ContactTagEFDeleter>();
        s.AddScoped<IReaderAsync<ContactTagIdDTO, ContactTagDomain>, ContactTagEFReader>();

        // TEST
        s.AddScoped<ICreatorAsync<TestDomain, NewTestDTO>, TestEFCreator>();
        s.AddScoped<IUpdaterAsync<TestDomain, TestUpdateDTO>, TestEFUpdater>();
        s.AddScoped<IDeleterAsync<ulong, TestDomain>, TestEFDeleter>();
        s.AddScoped<IReaderAsync<ulong, TestDomain>, TestEFReader>();
        s.AddScoped<IQuerierAsync<TestDomain, TestCriteriaDTO>>(
            sp => new TestEFQuerier(
                sp.GetRequiredService<EduZasDotnetContext>(),
                sp.GetRequiredService<IMapper<Test, TestDomain>>(),
                pageSize
            )
        );

        // Resource
        s.AddScoped<ICreatorAsync<ResourceDomain, NewResourceDTO>, ResourceEFCreator>();
        s.AddScoped<IUpdaterAsync<ResourceDomain, ResourceUpdateDTO>, ResourceEFUpdater>();
        s.AddScoped<IDeleterAsync<ulong, ResourceDomain>, ResourceEFDeleter>();
        s.AddScoped<IReaderAsync<ulong, ResourceDomain>, ResourceEFReader>();
        s.AddScoped<IQuerierAsync<ResourceDomain, ResourceCriteriaDTO>>(
            sp => new ResourceEFQuerier(
                sp.GetRequiredService<EduZasDotnetContext>(),
                sp.GetRequiredService<IMapper<Resource, ResourceDomain>>(),
                pageSize
            )
        );

        return s;
    }
}
