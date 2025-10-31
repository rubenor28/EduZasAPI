using Application.DTOs.Classes;
using Application.DTOs.ClassProfessors;
using Application.DTOs.ClassStudents;
using Application.DTOs.Notifications;
using Application.DTOs.Tags;
using Application.DTOs.Tests;
using Application.DTOs.UserNotifications;
using Application.DTOs.Users;
using Domain.Entities;
using EntityFramework.Application.DAOs.Classes;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers;
using InterfaceAdapters.Mappers.Common;

namespace MinimalAPI.Extensions;

/// <summary>
/// Métodos de extensión para registrar los servicios de base de datos en el contenedor de dependencias.
/// </summary>
public static class MapperServiceCollectionExtensions
{
    /// <summary>
    /// Registra el contexto de mapeo de datos en la configuración.
    /// </summary>
    /// <param name="services">La colección de servicios donde se registrará el contexto.</param>
    /// <returns>La colección de servicios con el contexto de base de datos registrado.</returns>
    public static IServiceCollection AddMapperServices(this IServiceCollection services)
    {
        // USERS
        services.AddSingleton<UserEFMapper>();
        services.AddSingleton<IMapper<User, UserDomain>>(s => s.GetRequiredService<UserEFMapper>());
        services.AddSingleton<IMapper<NewUserDTO, User>>(s => s.GetRequiredService<UserEFMapper>());
        services.AddSingleton<IUpdateMapper<UserUpdateDTO, User>>(s =>
            s.GetRequiredService<UserEFMapper>()
        );

        // CLASSES
        services.AddSingleton<ClassEFMapper>();
        services.AddSingleton<IMapper<NewClassDTO, Class>>(s =>
            s.GetRequiredService<ClassEFMapper>()
        );
        services.AddSingleton<IMapper<Class, ClassDomain>>(s =>
            s.GetRequiredService<ClassEFMapper>()
        );
        services.AddSingleton<IUpdateMapper<ClassUpdateDTO, Class>>(s =>
            s.GetRequiredService<ClassEFMapper>()
        );

        // CLASS PROFESSOR
        services.AddSingleton<ProfessorClassEFMapper>();
        services.AddSingleton<IMapper<ProfessorClassRelationDTO, ClassProfessor>>(s =>
            s.GetRequiredService<ProfessorClassEFMapper>()
        );
        services.AddSingleton<IMapper<ClassProfessor, ProfessorClassRelationDTO>>(s =>
            s.GetRequiredService<ProfessorClassEFMapper>()
        );
        services.AddSingleton<IUpdateMapper<ProfessorClassRelationDTO, ClassProfessor>>(s =>
            s.GetRequiredService<ProfessorClassEFMapper>()
        );

        // CLASS STUDENTS
        services.AddSingleton<StudentClassEFMapper>();
        services.AddSingleton<IMapper<StudentClassRelationDTO, ClassStudent>>(s =>
            s.GetRequiredService<StudentClassEFMapper>()
        );
        services.AddSingleton<IMapper<ClassStudent, StudentClassRelationDTO>>(s =>
            s.GetRequiredService<StudentClassEFMapper>()
        );
        services.AddSingleton<IUpdateMapper<StudentClassRelationDTO, ClassStudent>>(s =>
            s.GetRequiredService<StudentClassEFMapper>()
        );

        // NOTIFICATIONS
        services.AddSingleton<NotificationEFMapper>();
        services.AddSingleton<IMapper<Notification, NotificationDomain>>(s =>
            s.GetRequiredService<NotificationEFMapper>()
        );
        services.AddSingleton<IMapper<NewNotificationDTO, Notification>>(s =>
            s.GetRequiredService<NotificationEFMapper>()
        );

        // USER NOTIFICATIONS
        services.AddSingleton<UserNotificationEFMapper>();
        services.AddSingleton<IMapper<NotificationPerUser, UserNotificationDomain>>(s =>
            s.GetRequiredService<UserNotificationEFMapper>()
        );
        services.AddSingleton<IMapper<NewUserNotificationDTO, NotificationPerUser>>(s =>
            s.GetRequiredService<UserNotificationEFMapper>()
        );
        services.AddSingleton<IUpdateMapper<UserNotificationDomain, NotificationPerUser>>(s =>
            s.GetRequiredService<UserNotificationEFMapper>()
        );

        // TAGS
        services.AddSingleton<TagEFMapper>();
        services.AddSingleton<IMapper<Tag, TagDomain>>(s => s.GetRequiredService<TagEFMapper>());
        services.AddSingleton<IMapper<NewTagDTO, Tag>>(s => s.GetRequiredService<TagEFMapper>());

        // TESTS
        services.AddSingleton<TestEFMapper>();
        services.AddSingleton<IMapper<Test, TestDomain>>(s => s.GetRequiredService<TestEFMapper>());
        services.AddSingleton<IMapper<NewTestDTO, Test>>(s => s.GetRequiredService<TestEFMapper>());
        services.AddSingleton<IUpdateMapper<TestUpdateDTO, Test>>(s =>
            s.GetRequiredService<TestEFMapper>()
        );

        return services;
    }
}
