using Application.DTOs.Classes;
using Application.DTOs.ClassProfessors;
using Application.DTOs.ClassResources;
using Application.DTOs.ClassStudents;
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
using MinimalAPI.Application.DTOs.Classes;
using MinimalAPI.Application.DTOs.Common;
using MinimalAPI.Application.DTOs.Contacts;
using MinimalAPI.Application.DTOs.Resources;
using MinimalAPI.Application.DTOs.Tags;
using MinimalAPI.Application.DTOs.Users;
using MinimalAPI.Presentation.Mappers;

namespace MinimalAPI.Extensions;

/// <summary>
/// Métodos de extensión para registrar los servicios de base de datos en el contenedor de dependencias.
/// </summary>
public static class MapperServiceCollectionExtensions
{
    /// <summary>
    /// Registra un Mapper Bidireccional estándar (TIn <-> TOut).
    /// </summary>
    public static IServiceCollection RegisterBidirectionalMapper<TImplementation, TIn, TOut>(
        this IServiceCollection services
    )
        where TImplementation : class, IBidirectionalMapper<TIn, TOut>, IMapper<TOut, TIn>
    {
        // 1. Registramos la clase concreta (El "Source of Truth")
        services.AddSingleton<TImplementation>();

        // 2. Registramos la interfaz bidireccional apuntando a la concreta
        services.AddSingleton<IBidirectionalMapper<TIn, TOut>>(sp =>
            sp.GetRequiredService<TImplementation>()
        );

        // 3. Registramos la interfaz base de IDA (IMapper<TIn, TOut>) apuntando a la concreta
        services.AddSingleton<IMapper<TIn, TOut>>(sp => sp.GetRequiredService<TImplementation>());

        // 4. Registramos la interfaz de VUELTA (IMapper<TOut, TIn>) apuntando a la concreta
        services.AddSingleton<IMapper<TOut, TIn>>(sp => sp.GetRequiredService<TImplementation>());

        return services;
    }

    /// <summary>
    /// Registra un Mapper Bidireccional que retorna Result<> en la ida, pero valor plano en la vuelta.
    /// Específico para tu caso UserTypeIntMapper.
    /// </summary>
    public static IServiceCollection RegisterBidirectionalResultMapper<
        TImplementation,
        TIn,
        TOut,
        TError
    >(this IServiceCollection services)
        where TImplementation : class,
            IBidirectionalResultMapper<TIn, TOut, TError>,
            IMapper<TOut, TIn>
        where TError : notnull
    {
        // 1. Clase Concreta
        services.AddSingleton<TImplementation>();

        // 2. Interfaz Bidireccional con Result
        services.AddSingleton<IBidirectionalResultMapper<TIn, TOut, TError>>(sp =>
            sp.GetRequiredService<TImplementation>()
        );

        // 3. Interfaz Base de IDA (IMapper<TIn, Result<...>>)
        services.AddSingleton<IMapper<TIn, Result<TOut, TError>>>(sp =>
            sp.GetRequiredService<TImplementation>()
        );

        // 4. Interfaz de VUELTA (IMapper<TOut, TIn>) -> Aquí no hay Result, es plana
        services.AddSingleton<IMapper<TOut, TIn>>(sp => sp.GetRequiredService<TImplementation>());

        return services;
    }

    /// <summary>
    /// Registra el contexto de mapeo de datos en la configuración.
    /// </summary>
    /// <param name="s">La colección de servicios donde se registrará el contexto.</param>
    /// <returns>La colección de servicios con el contexto de base de datos registrado.</returns>
    public static IServiceCollection AddMapperServices(this IServiceCollection s)
    {
        // Errors
        s.AddSingleton<IMapper<UseCaseError, IResult>, UseCaseErrorMAPIMapper>();

        // String Query DTO
        s.RegisterBidirectionalResultMapper<StringSearchMapper, string, StringSearchType, Unit>();
        s.RegisterBidirectionalResultMapper<StringQueryMAPIMapper, StringQueryMAPI, StringQueryDTO, Unit>();
        s.RegisterBidirectionalResultMapper<OptionalStringQueryMAPIMapper, StringQueryMAPI?, StringQueryDTO?, Unit>();

        // USERS
        // EF
        s.RegisterBidirectionalResultMapper<UserTypeUintMapper, uint, UserType, Unit>();
        s.RegisterBidirectionalResultMapper<UserTypeUlongMapper, ulong, UserType, Unit>();
        s.RegisterBidirectionalResultMapper<UserTypeStringMapper, string, UserType, Unit>();
        s.RegisterBidirectionalResultMapper<OptionalUserTypeUintMapper, uint?, UserType?, Unit>();

        s.AddSingleton<IMapper<User, UserDomain>, UserMapper>();
        s.AddSingleton<IMapper<NewUserDTO, User>, NewUserEFMapper>();
        s.AddSingleton<IUpdateMapper<UserUpdateDTO, User>, UpdateUserEFMapper>();
        s.AddSingleton<IEFProjector<User, UserDomain, UserCriteriaDTO>, UserProjector>();

        // Minimal API
        s.AddSingleton<IMapper<UserDomain, PublicUserDTO>, UserMAPIMapper>();
        s.AddSingleton<IMapper<UserUpdateMAPI, Result<UserUpdateDTO, IEnumerable<FieldErrorDTO>>>, UserUpdateMAPIMapper>();
        s.RegisterBidirectionalResultMapper<UserCriteriaMAPIMapper, UserCriteriaMAPI, UserCriteriaDTO, IEnumerable<FieldErrorDTO>>();
        s.AddSingleton<IMapper<PaginatedQuery<UserDomain, UserCriteriaDTO>, PaginatedQuery<PublicUserDTO, UserCriteriaMAPI>>, UserSearchMAPIMapper>();

        // CLASSES
        // EF
        s.AddSingleton<IMapper<NewClassDTO, Class>, NewClassEFMapper>();
        s.AddSingleton<IUpdateMapper<ClassUpdateDTO, Class>, UpdateClassEFMapper>();
        s.AddSingleton<IMapper<Class, ClassDomain>, ClassMapper>();
        s.AddSingleton<IEFProjector<Class, ClassDomain, ClassCriteriaDTO>, ClassProjector>();

        //Minimal API
        s.RegisterBidirectionalResultMapper<ClassCriteriaMAPIMapper, ClassCriteriaMAPI, ClassCriteriaDTO, IEnumerable<FieldErrorDTO>>();
        s.AddSingleton<IMapper<PaginatedQuery<ClassDomain, ClassCriteriaDTO>, PaginatedQuery<ClassDomain, ClassCriteriaMAPI>>, ClassSearchMAPIMapper>();

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

        // Minimal API
        s.AddSingleton<IMapper<int, ulong, NotificationCriteriaDTO>, UserNotificationCriteriaMAPIMapper>();

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

        // Minimal API
        s.RegisterBidirectionalResultMapper<TagCriteriaMAPIMapper, TagCriteriaMAPI, TagCriteriaDTO, IEnumerable<FieldErrorDTO>>();
        s.AddSingleton<IMapper<PaginatedQuery<TagDomain, TagCriteriaDTO>, PaginatedQuery<string, TagCriteriaMAPI>>, TagSearchMAPIMapper>();

        // CONTACTS
        // EF
        s.AddSingleton<IMapper<AgendaContact, ContactDomain>, ContactMapper>();
        s.AddSingleton<IMapper<NewContactDTO, AgendaContact>, NewContactEFMapper>();
        s.AddSingleton<IUpdateMapper<ContactUpdateDTO, AgendaContact>, UpdateContactEFMapper>();
        s.AddSingleton<IEFProjector<AgendaContact, ContactDomain, ContactCriteriaDTO>, ContactProjector>();

        // Minimal API
        s.RegisterBidirectionalResultMapper<ContactCriteriaMAPIMapper, ContactCriteriaMAPI, ContactCriteriaDTO, IEnumerable<FieldErrorDTO>>();

        // CONTACT TAGS
        // EF
        s.AddSingleton<IMapper<ContactTag, ContactTagDomain>, ContactTagMapper>();
        s.AddSingleton<IMapper<NewContactTagDTO, ContactTag>, NewContactTagEFMapper>();

        // Minimal API
        s.AddSingleton<IMapper<ContactTagIdDTO, NewContactTagDTO>, NewContactTagMAPIMapper>();

        // TESTS
        // EF
        s.AddSingleton<IMapper<NewTestDTO, Test>, NewTestEFMapper>();
        s.AddSingleton<IUpdateMapper<TestUpdateDTO, Test>, UpdateTestEFMapper>();
        s.AddSingleton<IMapper<Test, TestDomain>, TestMapper>();
        s.AddSingleton<IEFProjector<Test, TestDomain, TestCriteriaDTO>, TestProjector>();

        // Resource
        // EF
        s.AddSingleton<IMapper<NewResourceDTO, Resource>, NewResourceEFMapper>();
        s.AddSingleton<IUpdateMapper<ResourceUpdateDTO, Resource>, UpdateResourceEFMapper>();
        s.AddSingleton<IMapper<Resource, ResourceDomain>, ResourceMapper>();
        s.AddSingleton<IEFProjector<Resource, ResourceDomain, ResourceCriteriaDTO>, ResourceProjector>();

        // Minimal API
        s.RegisterBidirectionalResultMapper<ResourceCriteriaMAPIMapper, ResourceCriteriaMAPI, ResourceCriteriaDTO, IEnumerable<FieldErrorDTO>>();
        s.AddSingleton<IMapper<PaginatedQuery<ResourceSummary, ResourceCriteriaDTO>, PaginatedQuery<ResourceSummary, ResourceCriteriaMAPI>>, ResourceSearchMAPIMapper>();

        // Class Resource
        // EF
        s.AddSingleton<IMapper<ClassResource, ClassResourceDomain>, ClassResourceMapper>();
        s.AddSingleton<IMapper<NewClassResourceDTO, ClassResource>, NewClassResourceEFMapper>();

        return s;
    }
}
