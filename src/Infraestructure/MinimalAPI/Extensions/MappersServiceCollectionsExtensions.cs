using Application.DTOs.Classes;
using Application.DTOs.ClassProfessors;
using Application.DTOs.ClassResources;
using Application.DTOs.ClassStudents;
using Application.DTOs.ClassTests;
using Application.DTOs.Common;
using Application.DTOs.Contacts;
using Application.DTOs.ContactTags;
using Application.DTOs.Notifications;
using Application.DTOs.Resources;
using Application.DTOs.Tags;
using Application.DTOs.Tests;
using Application.DTOs.UserNotifications;
using Application.DTOs.Users;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Classes;
using EntityFramework.InterfaceAdapters.Mappers.ClassProfessors;
using EntityFramework.InterfaceAdapters.Mappers.ClassResources;
using EntityFramework.InterfaceAdapters.Mappers.ClassStudents;
using EntityFramework.InterfaceAdapters.Mappers.ClassTests;
using EntityFramework.InterfaceAdapters.Mappers.Common;
using EntityFramework.InterfaceAdapters.Mappers.Contacts;
using EntityFramework.InterfaceAdapters.Mappers.ContactTags;
using EntityFramework.InterfaceAdapters.Mappers.Notifications;
using EntityFramework.InterfaceAdapters.Mappers.Resources;
using EntityFramework.InterfaceAdapters.Mappers.Tags;
using EntityFramework.InterfaceAdapters.Mappers.Tests;
using EntityFramework.InterfaceAdapters.Mappers.UserNotifications;
using EntityFramework.InterfaceAdapters.Mappers.Users;
using InterfaceAdapters.Mappers.Common;
using InterfaceAdapters.Mappers.Users;

namespace MinimalAPI.Extensions;

/// <summary>
/// Métodos de extensión para registrar los servicios de mapeo en el contenedor de dependencias.
/// </summary>
public static class MapperServiceCollectionExtensions
{
    /// <summary>
    /// Registra un mapeador bidireccional que devuelve resultados (Result pattern).
    /// </summary>
    /// <typeparam name="TImplementation">Tipo de la implementación del mapeador.</typeparam>
    /// <typeparam name="TIn">Tipo de entrada.</typeparam>
    /// <typeparam name="TOut">Tipo de salida.</typeparam>
    /// <typeparam name="TError">Tipo de error.</typeparam>
    /// <param name="services">Colección de servicios.</param>
    /// <returns>La colección de servicios actualizada.</returns>
    private static IServiceCollection RegisterBidirectionalResultMappers<
        TImplementation,
        TIn,
        TOut,
        TError
    >(this IServiceCollection services)
        where TImplementation : class, IMapper<TIn, Result<TOut, TError>>, IMapper<TOut, TIn>
        where TError : notnull
    {
        services.AddSingleton<TImplementation>();
        services.AddSingleton<IMapper<TIn, Result<TOut, TError>>>(sp =>
            sp.GetRequiredService<TImplementation>()
        );
        services.AddSingleton<IMapper<TOut, TIn>>(sp => sp.GetRequiredService<TImplementation>());
        return services;
    }

    /// <summary>
    /// Registra todos los servicios de mapeo de la aplicación.
    /// </summary>
    /// <param name="s">Colección de servicios.</param>
    /// <returns>La colección de servicios actualizada.</returns>
    public static IServiceCollection AddMapperServices(this IServiceCollection s)
    {
        // USERS
        // EF
        s.RegisterBidirectionalResultMappers<UserTypeUintMapper, uint, UserType, Unit>();

        s.AddSingleton<IMapper<User, UserDomain>, UserMapper>();
        s.AddSingleton<IMapper<NewUserDTO, User>, NewUserEFMapper>();
        s.AddSingleton<IUpdateMapper<UserUpdateDTO, User>, UpdateUserEFMapper>();
        s.AddSingleton<IEFProjector<User, UserDomain, UserCriteriaDTO>, UserProjector>();

        // MinimalAPI
        s.AddSingleton<IMapper<UserDomain, PublicUserDTO>, PublicUserMapper>();
        s.AddSingleton<IMapper<PaginatedQuery<UserDomain, UserCriteriaDTO>, PaginatedQuery<PublicUserDTO, UserCriteriaDTO>>, UserSearchMapper>();

        // CLASSES
        // EF
        s.AddSingleton<IMapper<NewClassDTO, Class>, NewClassEFMapper>();
        s.AddSingleton<IUpdateMapper<ClassUpdateDTO, Class>, UpdateClassEFMapper>();
        s.AddSingleton<IMapper<Class, ClassDomain>, ClassMapper>();
        s.AddSingleton<IEFProjector<Class, ClassDomain, ClassCriteriaDTO>, ClassProjector>();

        // CLASS PROFESSOR
        // EF
        s.AddSingleton<IMapper<NewClassProfessorDTO, ClassProfessor>, NewClassProfessorEFMapper>();
        s.AddSingleton<IUpdateMapper<ClassProfessorUpdateDTO, ClassProfessor>, UpdateClassProfessorEFMapper>();
        s.AddSingleton<IMapper<ClassProfessor, ClassProfessorDomain>, ClassProfessorMapper>();
        s.AddSingleton<IEFProjector<ClassProfessor, ClassProfessorDomain, ClassProfessorCriteriaDTO>, ClassProfessorProjector>();

        // CLASS STUDENTS
        // EF
        s.AddSingleton<IMapper<UserClassRelationId, ClassStudent>, NewClassStudentEFMapper>();
        s.AddSingleton<IUpdateMapper<ClassStudentUpdateDTO, ClassStudent>, UpdateClassStudentEFMapper>();
        s.AddSingleton<IMapper<ClassStudent, ClassStudentDomain>, ClassStudentMapper>();

        // NOTIFICATIONS
        // EF
        s.AddSingleton<IMapper<Notification, NotificationDomain>, NotificationMapper>();
        s.AddSingleton<IMapper<NewNotificationDTO, Notification>, NewNotificationEFMapper>();
        s.AddSingleton<IEFProjector<Notification, NotificationDomain, NotificationCriteriaDTO>, NotificationProjector>();

        // USER NOTIFICATIONS
        // EF
        s.AddSingleton<IMapper<NotificationPerUser, UserNotificationDomain>, UserNotificationMapper>();
        s.AddSingleton<IMapper<NewUserNotificationDTO, NotificationPerUser>, NewUserNotificationEFMapper>();
        s.AddSingleton<IUpdateMapper<UserNotificationUpdateDTO, NotificationPerUser>, UpdateUserNotificationEFMapper>();

        // TAGS
        // EF
        s.AddSingleton<IMapper<Tag, TagDomain>, TagMapper>();
        s.AddSingleton<IMapper<NewTagDTO, Tag>, NewTagEFMapper>();
        s.AddSingleton<IEFProjector<Tag, TagDomain, TagCriteriaDTO>, TagProjector>();

        // CONTACTS
        // EF
        s.AddSingleton<IMapper<AgendaContact, ContactDomain>, ContactMapper>();
        s.AddSingleton<IMapper<NewContactDTO, AgendaContact>, NewContactEFMapper>();
        s.AddSingleton<IUpdateMapper<ContactUpdateDTO, AgendaContact>, UpdateContactEFMapper>();
        s.AddSingleton<IEFProjector<AgendaContact, ContactDomain, ContactCriteriaDTO>, ContactProjector>();

        // CONTACT TAGS
        // EF
        s.AddSingleton<IMapper<ContactTag, ContactTagDomain>, ContactTagMapper>();
        s.AddSingleton<IMapper<NewContactTagDTO, ContactTag>, NewContactTagEFMapper>();

        // TESTS
        // EF
        s.AddSingleton<IMapper<NewTestDTO, Test>, NewTestEFMapper>();
        s.AddSingleton<IUpdateMapper<TestUpdateDTO, Test>, UpdateTestEFMapper>();
        s.AddSingleton<IMapper<Test, TestDomain>, TestMapper>();
        s.AddSingleton<IEFProjector<Test, TestDomain, TestCriteriaDTO>, TestProjector>();
        s.AddSingleton<IEFProjector<Test, TestSummary, TestCriteriaDTO>, TestSummaryProjector>();
        s.AddSingleton<IEFProjector<Class, ClassTestAssociationDTO, ClassTestAssociationCriteriaDTO>, ClassTestAssociationProjector>();
        
        // Class Tests
        s.AddSingleton<IMapper<ClassTestDTO, TestPerClass>, NewClassTestEFMapper>();
        s.AddSingleton<IUpdateMapper<ClassTestDTO, TestPerClass>, UpdateClassTestEFMapper>();
        s.AddSingleton<IMapper<TestPerClass, ClassTestDomain>, ClassTestMapper>();

        // Resource
        // EF
        s.AddSingleton<IMapper<NewResourceDTO, Resource>, NewResourceEFMapper>();
        s.AddSingleton<IUpdateMapper<ResourceUpdateDTO, Resource>, UpdateResourceEFMapper>();
        s.AddSingleton<IMapper<Resource, ResourceDomain>, ResourceMapper>();
        s.AddSingleton<IEFProjector<Resource, ResourceDomain, ResourceCriteriaDTO>, ResourceProjector>();
        s.AddSingleton<IEFProjector<Resource, ResourceSummary, ResourceCriteriaDTO>, ResourceSummaryProjector>();

        // Class Resource
        // EF
        s.AddSingleton<IMapper<ClassResource, ClassResourceDomain>, ClassResourceMapper>();
        s.AddSingleton<IMapper<ClassResourceDTO, ClassResource>, NewClassResourceEFMapper>();
        s.AddSingleton<IUpdateMapper<ClassResourceDTO, ClassResource>, ClassResourceUpdateMapper>();
        s.AddSingleton<IEFProjector<Class, ClassResourceAssociationDTO, ClassResourceAssociationCriteriaDTO>, ClassResourceAssociationProjector>();

        return s;
    }
}
