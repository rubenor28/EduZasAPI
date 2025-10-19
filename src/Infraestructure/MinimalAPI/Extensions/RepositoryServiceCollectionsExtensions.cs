using Application.DAOs;
using Application.DTOs.Classes;
using Application.DTOs.ClassProfessors;
using Application.DTOs.ClassStudents;
using Application.DTOs.Notifications;
using Application.DTOs.UserNotifications;
using Application.DTOs.Users;
using Domain.Entities;
using EntityFramework.Application.DAOs.Classes;
using EntityFramework.Application.DAOs.ClassProfessors;
using EntityFramework.Application.DAOs.ClassStudents;
using EntityFramework.Application.DAOs.Notifications;
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
        services.AddScoped<IQuerierAsync<UserDomain, UserCriteriaDTO>>(s => new UserEFQuerier(
            s.GetService<EduZasDotnetContext>()!,
            s.GetService<IMapper<User, UserDomain>>()!,
            pageSize
        ));

        // CLASSES
        services.AddScoped<ICreatorAsync<ClassDomain, NewClassDTO>, ClassEFCreator>();
        services.AddScoped<IUpdaterAsync<ClassDomain, ClassUpdateDTO>, ClassEFUpdater>();
        services.AddScoped<IReaderAsync<string, ClassDomain>, ClassEFReader>();
        services.AddScoped<IDeleterAsync<string, ClassDomain>, ClassEFDeleter>();
        services.AddScoped<IQuerierAsync<ClassDomain, ClassCriteriaDTO>>(s => new ClassEFQuerier(
            s.GetRequiredService<EduZasDotnetContext>(),
            s.GetRequiredService<IMapper<Class, ClassDomain>>(),
            pageSize
        ));

        // CLASS PROFESSORS
        services.AddScoped<
            ICreatorAsync<ProfessorClassRelationDTO, ProfessorClassRelationDTO>,
            ClassProfessorEFCreator
        >();
        services.AddScoped<
            IUpdaterAsync<ProfessorClassRelationDTO, ProfessorClassRelationDTO>,
            ClassProfessorsEFUpdater
        >();
        services.AddScoped<
            IReaderAsync<ClassUserRelationIdDTO, ProfessorClassRelationDTO>,
            ClassProfessorsEFReader
        >();
        services.AddScoped<
            IDeleterAsync<ClassUserRelationIdDTO, ProfessorClassRelationDTO>,
            ClassProfessorsEFDeleter
        >();
        services.AddScoped<
            IQuerierAsync<ProfessorClassRelationDTO, ProfessorClassRelationCriteriaDTO>
        >(s => new ClassProfessorsEFQuerier(
            s.GetRequiredService<EduZasDotnetContext>(),
            s.GetRequiredService<IMapper<ClassProfessor, ProfessorClassRelationDTO>>(),
            pageSize
        ));

        // CLASS STUDENTS
        services.AddScoped<
            ICreatorAsync<StudentClassRelationDTO, StudentClassRelationDTO>,
            ClassStudentEFCreator
        >();
        services.AddScoped<
            IReaderAsync<ClassUserRelationIdDTO, StudentClassRelationDTO>,
            ClassStudentsEFReader
        >();
        services.AddScoped<
            IUpdaterAsync<StudentClassRelationDTO, StudentClassRelationDTO>,
            ClassStudentsEFUpdater
        >();
        services.AddScoped<
            IDeleterAsync<ClassUserRelationIdDTO, StudentClassRelationDTO>,
            ClassStudentsEFDeleter
        >();
        services.AddScoped<IQuerierAsync<StudentClassRelationDTO, StudentClassRelationCriteriaDTO>>(
            s => new ClassStudentEFQuerier(
                s.GetRequiredService<EduZasDotnetContext>(),
                s.GetRequiredService<IMapper<ClassStudent, StudentClassRelationDTO>>(),
                pageSize
            )
        );

        // NOTIFICATIONS
        services.AddScoped<IReaderAsync<ulong, NotificationDomain>, NotificationEFReader>();
        services.AddScoped<
            ICreatorAsync<NotificationDomain, NewNotificationDTO>,
            NotificationEFCreator
        >();
        services.AddScoped<IQuerierAsync<NotificationDomain, NotificationCriteriaDTO>>(
            s => new NotificationEFQuerier(
                s.GetRequiredService<EduZasDotnetContext>(),
                s.GetRequiredService<IMapper<Notification, NotificationDomain>>(),
                pageSize
            )
        );

        // USER NOTIFICATIONS
        services.AddScoped<
            ICreatorAsync<UserNotificationDomain, NewUserNotificationDTO>,
            UserNotificationEFCreator
        >();
        services.AddScoped<
            IUpdaterAsync<UserNotificationDomain, UserNotificationDomain>,
            UserNotificationEFUpdater
        >();
        services.AddScoped<
            IReaderAsync<UserNotificationIdDTO, UserNotificationDomain>,
            UserNotificationEFReader
        >();

        return services;
    }
}
