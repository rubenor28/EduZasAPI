using Application.DAOs;
using Application.DTOs.Classes;
using Application.DTOs.ClassProfessors;
using Application.DTOs.ClassResources;
using Application.DTOs.ClassStudents;
using Application.DTOs.ClassTests;
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
using EntityFramework.Application.DAOs.ClassResources;
using EntityFramework.Application.DAOs.ClassStudents;
using EntityFramework.Application.DAOs.ClassTests;
using EntityFramework.Application.DAOs.Contacts;
using EntityFramework.Application.DAOs.ContactTags;
using EntityFramework.Application.DAOs.Notifications;
using EntityFramework.Application.DAOs.Resources;
using EntityFramework.Application.DAOs.Tags;
using EntityFramework.Application.DAOs.Tests;
using EntityFramework.Application.DAOs.UserNotifications;
using EntityFramework.Application.DAOs.Users;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Composition.Extensions;

/// <summary>
/// Métodos de extensión para registrar los repositorios en el contenedor de dependencias.
/// </summary>
internal static class RepositoryServiceCollectionExtensions
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
                sp.GetRequiredService<IEFProjector<User, UserDomain, UserCriteriaDTO>>(),
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
                sp.GetRequiredService<IEFProjector<Class, ClassDomain, ClassCriteriaDTO>>(),
                pageSize
             )
        );
        s.AddScoped<IQuerierAsync<ProfessorClassesSummaryDTO, ProfessorClassesSummaryCriteriaDTO>>(
            sp => new ProfessorClassesEFQuerier(
                sp.GetRequiredService<EduZasDotnetContext>(),
                sp.GetRequiredService<IEFProjector<Class, ProfessorClassesSummaryDTO, ProfessorClassesSummaryCriteriaDTO>>(),
                pageSize
             )
        );
        s.AddScoped<IQuerierAsync<StudentClassesSummaryDTO, StudentClassesSummaryCriteriaDTO>>(
            sp => new StudentClassesSummaryEFQuerier(
                sp.GetRequiredService<EduZasDotnetContext>(),
                sp.GetRequiredService<IEFProjector<Class, StudentClassesSummaryDTO, StudentClassesSummaryCriteriaDTO>>(),
                pageSize
             )
        );

        // CLASS PROFESSORS
        s.AddScoped<ICreatorAsync<ClassProfessorDomain, NewClassProfessorDTO>, ClassProfessorsEFCreator>();
        s.AddScoped<IBulkCreatorAsync<ClassProfessorDomain, NewClassProfessorDTO>, ClassProfessorsEFCreator>();
        s.AddScoped<IUpdaterAsync<ClassProfessorDomain, ClassProfessorUpdateDTO>, ClassProfessorsEFUpdater>();
        s.AddScoped<IReaderAsync<UserClassRelationId, ClassProfessorDomain>, ClassProfessorsEFReader>();
        s.AddScoped<IDeleterAsync<UserClassRelationId, ClassProfessorDomain>, ClassProfessorsEFDeleter>();

        // CLASS STUDENTS
        s.AddScoped<ICreatorAsync<ClassStudentDomain, UserClassRelationId>, ClassStudentEFCreator>();
        s.AddScoped<IReaderAsync<UserClassRelationId, ClassStudentDomain>, ClassStudentsEFReader>();
        s.AddScoped<IUpdaterAsync<ClassStudentDomain, ClassStudentUpdateDTO>, ClassStudentsEFUpdater>();
        s.AddScoped<IDeleterAsync<UserClassRelationId, ClassStudentDomain>, ClassStudentsEFDeleter>();
        s.AddScoped<IQuerierAsync<ClassProfessorSummaryDTO, ClassProfessorSummaryCriteriaDTO>>(
            sp => new ClassProfessorSummaryEFQuerier(
                sp.GetRequiredService<EduZasDotnetContext>(),
                sp.GetRequiredService<IEFProjector<User, ClassProfessorSummaryDTO, ClassProfessorSummaryCriteriaDTO>>(),
                pageSize
             )
        );

        // NOTIFICATIONS
        s.AddScoped<NotificationEFCreator>();
        s.AddScoped<ICreatorAsync<NotificationDomain, NewNotificationDTO>>(sp => sp.GetRequiredService<NotificationEFCreator>());
        s.AddScoped<IBulkCreatorAsync<NotificationDomain, NewNotificationDTO>>(sp => sp.GetRequiredService<NotificationEFCreator>());
        s.AddScoped<IReaderAsync<ulong, NotificationDomain>, NotificationEFReader>();
        s.AddScoped<IQuerierAsync<NotificationDomain, NotificationCriteriaDTO>>(
            sp => new NotificationEFQuerier(
                sp.GetRequiredService<EduZasDotnetContext>(),
                sp.GetRequiredService<IEFProjector<Notification, NotificationDomain, NotificationCriteriaDTO>>(),
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
                sp.GetRequiredService<IEFProjector<Tag, TagDomain, TagCriteriaDTO>>(),
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
                sp.GetRequiredService<IEFProjector<AgendaContact, ContactDomain, ContactCriteriaDTO>>(),
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
        s.AddScoped<IDeleterAsync<Guid, TestDomain>, TestEFDeleter>();
        s.AddScoped<IReaderAsync<Guid, TestDomain>, TestEFReader>();
        s.AddScoped<IQuerierAsync<TestDomain, TestCriteriaDTO>>(
            sp => new TestEFQuerier(
                sp.GetRequiredService<EduZasDotnetContext>(),
                sp.GetRequiredService<IEFProjector<Test, TestDomain, TestCriteriaDTO>>(),
                pageSize
            )
        );
        s.AddScoped<IQuerierAsync<TestSummary, TestCriteriaDTO>>(
            sp => new TestSummaryEFQuerier(
                sp.GetRequiredService<EduZasDotnetContext>(),
                sp.GetRequiredService<IEFProjector<Test, TestSummary, TestCriteriaDTO>>(),
                pageSize
            )
        );

        // Class Tests
        s.AddScoped<IReaderAsync<ClassTestIdDTO, ClassTestDomain>, ClassTestEFReader>();
        s.AddScoped<ICreatorAsync<ClassTestDomain, ClassTestDTO>, ClassTestEFCreator>();
        s.AddScoped<IUpdaterAsync<ClassTestDomain, ClassTestDTO>, ClassTestEFUpdater>();
        s.AddScoped<IDeleterAsync<ClassTestIdDTO, ClassTestDomain>, ClassTestEFDeleter>();
        s.AddScoped<IQuerierAsync<ClassTestAssociationDTO, ClassTestAssociationCriteriaDTO>>(
            sp => new ClassTestAssociationEFQuerier(
                sp.GetRequiredService<EduZasDotnetContext>(),
                sp.GetRequiredService<IEFProjector<Class, ClassTestAssociationDTO, ClassTestAssociationCriteriaDTO>>(),
                pageSize
            )
        );

        // Resource
        s.AddScoped<ICreatorAsync<ResourceDomain, NewResourceDTO>, ResourceEFCreator>();
        s.AddScoped<IUpdaterAsync<ResourceDomain, ResourceUpdateDTO>, ResourceEFUpdater>();
        s.AddScoped<IDeleterAsync<Guid, ResourceDomain>, ResourceEFDeleter>();
        s.AddScoped<IReaderAsync<Guid, ResourceDomain>, ResourceEFReader>();
        s.AddScoped<IQuerierAsync<ResourceSummary, ResourceCriteriaDTO>>(
            sp => new ResourceSummaryEFQuerier(
                sp.GetRequiredService<EduZasDotnetContext>(),
                sp.GetRequiredService<IEFProjector<Resource, ResourceSummary, ResourceCriteriaDTO>>(),
                pageSize
            )
        );

        // Class Resource
        s.AddScoped<ICreatorAsync<ClassResourceDomain, ClassResourceDTO>, ClassResourceEFCreator>();
        s.AddScoped<IDeleterAsync<ClassResourceIdDTO, ClassResourceDomain>, ClassResourceEFDeleter>();
        s.AddScoped<IUpdaterAsync<ClassResourceDomain, ClassResourceDTO>, ClassResourceEFUpdater>();
        s.AddScoped<IReaderAsync<ClassResourceIdDTO, ClassResourceDomain>, ClassResourceEFReader>();
        s.AddScoped<IQuerierAsync<ClassResourceAssociationDTO, ClassResourceAssociationCriteriaDTO>>(
            sp => new ClassResourceAssociationEFQuerier(
                sp.GetRequiredService<EduZasDotnetContext>(),
                sp.GetRequiredService<IEFProjector<Class, ClassResourceAssociationDTO, ClassResourceAssociationCriteriaDTO>>(),
                pageSize
            )
        );

        return s;
    }
}
