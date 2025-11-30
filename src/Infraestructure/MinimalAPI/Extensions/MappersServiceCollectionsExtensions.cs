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
using MinimalAPI.Application.DTOs.ClassProfessors;
using MinimalAPI.Application.DTOs.ClassResources;
using MinimalAPI.Application.DTOs.ClassStudents;
using MinimalAPI.Application.DTOs.Common;
using MinimalAPI.Application.DTOs.Contacts;
using MinimalAPI.Application.DTOs.Notifications;
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
        where TOut : notnull
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

    public static IServiceCollection RegisterEFProjector<TImp, TEntity, TEF>(
        this IServiceCollection services
    )
        where TImp : class, IEFProjector<TEF, TEntity>
    {
        // 1. Clase Concreta
        services.AddSingleton<TImp>();

        services.AddSingleton<IMapper<TEF, TEntity>>(sp => sp.GetRequiredService<TImp>());
        services.AddSingleton<IEFProjector<TEF, TEntity>>(sp => sp.GetRequiredService<TImp>());

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
        s.RegisterBidirectionalResultMapper<
            StringQueryMAPIMapper,
            StringQueryMAPI,
            StringQueryDTO,
            Unit
        >();
        s.RegisterBidirectionalResultMapper<
            OptionalStringQueryMAPIMapper,
            StringQueryMAPI?,
            Optional<StringQueryDTO>,
            Unit
        >();

        // USERS
        // EF
        s.RegisterBidirectionalResultMapper<UserTypeUintMapper, uint, UserType, Unit>();
        s.RegisterBidirectionalResultMapper<UserTypeUlongMapper, ulong, UserType, Unit>();
        s.RegisterBidirectionalResultMapper<UserTypeStringMapper, string, UserType, Unit>();
        s.RegisterBidirectionalResultMapper<
            OptionalUserTypeUintMapper,
            uint?,
            Optional<UserType>,
            Unit
        >();
        s.AddSingleton<IMapper<NewUserDTO, User>, NewUserEFMapper>();
        s.AddSingleton<IUpdateMapper<UserUpdateDTO, User>, UpdateUserEFMapper>();
        s.RegisterEFProjector<UserProjector, UserDomain, User>();

        // Minimal API
        s.AddSingleton<IMapper<Executor, ReadUserDTO>, UserReadMAPIMapper>();
        s.AddSingleton<IMapper<UserDomain, PublicUserMAPI>, UserMAPIMapper>();
        s.AddSingleton<IMapper<NewUserMAPI, NewUserDTO>, NewUserMAPIMapper>();
        s.AddSingleton<IMapper<ulong, Executor, DeleteUserDTO>, DeleteUserMAPIMapper>();
        s.AddSingleton<
            IMapper<UserUpdateMAPI, Executor, Result<UserUpdateDTO, IEnumerable<FieldErrorDTO>>>,
            UserUpdateMAPIMapper
        >();
        s.RegisterBidirectionalResultMapper<
            UserCriteriaMAPIMapper,
            UserCriteriaMAPI,
            UserCriteriaDTO,
            IEnumerable<FieldErrorDTO>
        >();
        s.AddSingleton<
            IMapper<
                PaginatedQuery<UserDomain, UserCriteriaDTO>,
                PaginatedQuery<PublicUserMAPI, UserCriteriaMAPI>
            >,
            UserSearchMAPIMapper
        >();

        // CLASSES
        // EF
        s.AddSingleton<IMapper<NewClassDTO, Class>, NewClassEFMapper>();
        s.AddSingleton<IUpdateMapper<ClassUpdateDTO, Class>, UpdateClassEFMapper>();
        s.RegisterEFProjector<ClassProjector, ClassDomain, Class>();

        //Minimal API
        s.AddSingleton<IMapper<ClassDomain, PublicClassMAPI>, ClassMAPIMapper>();
        s.AddSingleton<IMapper<NewClassMAPI, Executor, NewClassDTO>, NewClassMAPIMapper>();
        s.AddSingleton<IMapper<string, Executor, DeleteClassDTO>, DeleteClassMAPIMapper>();
        s.AddSingleton<IMapper<ClassUpdateMAPI, Executor, ClassUpdateDTO>, ClassUpdateMAPIMapper>();
        s.RegisterBidirectionalMapper<
            WithStudentMAPIMapper,
            WithStudentMAPI?,
            Optional<WithStudentDTO>
        >();
        s.RegisterBidirectionalMapper<
            WithProfessorMAPIMapper,
            WithProfessorMAPI?,
            Optional<WithProfessorDTO>
        >();
        s.RegisterBidirectionalResultMapper<
            ClassCriteriaMAPIMapper,
            ClassCriteriaMAPI,
            ClassCriteriaDTO,
            IEnumerable<FieldErrorDTO>
        >();
        s.AddSingleton<
            IMapper<
                PaginatedQuery<ClassDomain, ClassCriteriaDTO>,
                PaginatedQuery<PublicClassMAPI, ClassCriteriaMAPI>
            >,
            ClassSearchMAPIMapper
        >();

        // CLASS PROFESSOR
        // EF
        s.AddSingleton<IMapper<NewClassProfessorDTO, ClassProfessor>, NewClassProfessorEFMapper>();
        s.AddSingleton<
            IUpdateMapper<ClassProfessorUpdateDTO, ClassProfessor>,
            UpdateClassProfessorEFMapper
        >();
        s.RegisterEFProjector<ClassProfessorProjector, ClassProfessorDomain, ClassProfessor>();

        // Minimal API
        s.AddSingleton<
            IMapper<ClassProfessorDomain, ClassProfessorMAPI>,
            ClassProfessorMAPIMapper
        >();
        s.AddSingleton<
            IMapper<ClassProfessorMAPI, Executor, NewClassProfessorDTO>,
            NewClassProfessorMAPIMapper
        >();
        s.AddSingleton<
            IMapper<string, ulong, Executor, DeleteClassProfessorDTO>,
            DeleteClassProfessorMAPIMapper
        >();
        s.AddSingleton<
            IMapper<ClassProfessorMAPI, Executor, ClassProfessorUpdateDTO>,
            ClassProfessorUpdateMAPIMapper
        >();
        s.AddSingleton<
            IMapper<
                PaginatedQuery<ClassProfessorDomain, ClassProfessorCriteriaDTO>,
                PaginatedQuery<ClassProfessorMAPI, ClassProfessorCriteriaMAPI>
            >,
            ClassProfessorSearchMAPIMapper
        >();
        s.RegisterBidirectionalMapper<
            ClassProfessorsCriteriaMAPIMapper,
            ClassProfessorCriteriaMAPI,
            ClassProfessorCriteriaDTO
        >();

        // CLASS STUDENTS
        // EF
        s.AddSingleton<IMapper<NewClassStudentDTO, ClassStudent>, NewClassStudentEFMapper>();
        s.AddSingleton<
            IUpdateMapper<ClassStudentUpdateDTO, ClassStudent>,
            UpdateClassStudentEFMapper
        >();
        s.RegisterEFProjector<ClassStudentProjector, ClassStudentDomain, ClassStudent>();

        // Minimal API
        s.AddSingleton<
            IMapper<EnrollClassMAPI, Executor, NewClassStudentDTO>,
            NewClassStudentMAPIMapper
        >();
        s.AddSingleton<
            IMapper<string, ulong, Executor, DeleteClassStudentDTO>,
            DeleteClassStudentMAPIMapper
        >();
        s.AddSingleton<
            IMapper<ClassStudentUpdateMAPI, Executor, ClassStudentUpdateDTO>,
            UpdateClassStudentMAPIMapper
        >();

        // NOTIFICATIONS
        // EF
        s.RegisterEFProjector<NotificationProjector, NotificationDomain, Notification>();
        s.AddSingleton<IMapper<NewNotificationDTO, Notification>, NewNotificationEFMapper>();

        // Minimal API
        s.RegisterBidirectionalMapper<
            NotificationCriteriaMAPIMapper,
            NotificationCriteriaMAPI,
            NotificationCriteriaDTO
        >();
        s.AddSingleton<
            IMapper<int, ulong, NotificationCriteriaDTO>,
            UserNotificationCriteriaMAPIMapper
        >();
        s.AddSingleton<
            IMapper<NotificationDomain, PublicNotificationMAPI>,
            PublicNotificationMAPIMapper
        >();
        s.AddSingleton<
            IMapper<
                PaginatedQuery<NotificationDomain, NotificationCriteriaDTO>,
                PaginatedQuery<PublicNotificationMAPI, NotificationCriteriaMAPI>
            >,
            NotificationSearchMAPIMapper
        >();

        // USER NOTIFICATIONS
        // EF
        s.AddSingleton<
            IMapper<NewUserNotificationDTO, NotificationPerUser>,
            NewUserNotificationEFMapper
        >();
        s.AddSingleton<
            IUpdateMapper<UserNotificationUpdateDTO, NotificationPerUser>,
            UpdateUserNotificationEFMapper
        >();
        s.RegisterEFProjector<
            UserNotificationProjector,
            UserNotificationDomain,
            NotificationPerUser
        >();

        // TAGS
        // EF
        s.RegisterEFProjector<TagProjector, TagDomain, Tag>();
        s.AddSingleton<IMapper<NewTagDTO, Tag>, NewTagEFMapper>();

        // Minimal API
        s.RegisterBidirectionalResultMapper<
            TagCriteriaMAPIMapper,
            TagCriteriaMAPI,
            TagCriteriaDTO,
            IEnumerable<FieldErrorDTO>
        >();
        s.AddSingleton<IMapper<TagDomain, PublicTagMAPI>, PublicTagMAPIMapper>();
        s.AddSingleton<
            IMapper<
                PaginatedQuery<TagDomain, TagCriteriaDTO>,
                PaginatedQuery<string, TagCriteriaMAPI>
            >,
            TagSearchMAPIMapper
        >();

        // CONTACTS
        // EF
        s.AddSingleton<IMapper<NewContactDTO, AgendaContact>, NewContactEFMapper>();
        s.AddSingleton<IUpdateMapper<ContactUpdateDTO, AgendaContact>, UpdateContactEFMapper>();
        s.RegisterEFProjector<ContactProjector, ContactDomain, AgendaContact>();

        // Minimal API
        s.AddSingleton<IMapper<ContactDomain, PublicContactMAPI>, PublicContactMAPIMapper>();
        s.RegisterBidirectionalResultMapper<
            ContactCriteriaMAPIMapper,
            ContactCriteriaMAPI,
            ContactCriteriaDTO,
            IEnumerable<FieldErrorDTO>
        >();
        s.AddSingleton<IMapper<ContactUpdateMAPI, ContactUpdateDTO>, ContactUpdateMAPIMapper>();
        s.AddSingleton<
            IMapper<
                PaginatedQuery<ContactDomain, ContactCriteriaDTO>,
                PaginatedQuery<PublicContactMAPI, ContactCriteriaMAPI>
            >,
            ContactSearchMAPIMapper
        >();
        s.AddSingleton<IMapper<NewContactMAPI, Executor, NewContactDTO>, NewContactMAPIMapper>();
        s.AddSingleton<
            IMapper<ulong, ulong, Executor, DeleteContactDTO>,
            DeleteContactMAPIMapper
        >();

        // CONTACT TAGS
        // EF
        s.RegisterEFProjector<ContactTagProjector, ContactTagDomain, ContactTag>();
        s.AddSingleton<IMapper<NewContactTagDTO, ContactTag>, NewContactTagEFMapper>();

        // Minimal API
        s.AddSingleton<
            IMapper<ContactTagIdDTO, Executor, NewContactTagDTO>,
            NewContactTagMAPIMapper
        >();
        s.AddSingleton<
            IMapper<ulong, ulong, string, Executor, DeleteContactTagDTO>,
            DeleteContactTagMAPIMapper
        >();

        // TESTS
        // EF
        s.AddSingleton<IMapper<NewTestDTO, Test>, NewTestEFMapper>();
        s.AddSingleton<IUpdateMapper<TestUpdateDTO, Test>, UpdateTestEFMapper>();
        s.RegisterEFProjector<TestProjector, TestDomain, Test>();

        // Resource
        // EF
        s.AddSingleton<IMapper<NewResourceDTO, Resource>, NewResourceEFMapper>();
        s.AddSingleton<IUpdateMapper<ResourceUpdateDTO, Resource>, UpdateResourceEFMapper>();
        s.RegisterEFProjector<ResourceProjector, ResourceDomain, Resource>();
        s.RegisterEFProjector<ResourceSummaryProjector, ResourceSummary, Resource>();

        // Minimal API
        s.RegisterBidirectionalResultMapper<
            ResourceCriteriaMAPIMapper,
            ResourceCriteriaMAPI,
            ResourceCriteriaDTO,
            IEnumerable<FieldErrorDTO>
        >();
        s.AddSingleton<IMapper<NewResourceMAPI, Executor, NewResourceDTO>, NewResourceMAPIMapper>();
        s.AddSingleton<IMapper<ResourceDomain, PublicResourceMAPI>, PublicResourceMAPIMapper>();
        s.AddSingleton<IMapper<Guid, Executor, DeleteResourceDTO>, DeleteResourceMAPIMapper>();
        s.AddSingleton<
            IMapper<ResourceUpdateMAPI, Executor, ResourceUpdateDTO>,
            ResourceUpdateMAPIMapper
        >();
        s.AddSingleton<
            IMapper<
                PaginatedQuery<ResourceSummary, ResourceCriteriaDTO>,
                PaginatedQuery<ResourceSummary, ResourceCriteriaMAPI>
            >,
            ResourceSearchMAPIMapper
        >();

        // Class Resource
        // EF
        s.AddSingleton<IMapper<NewClassResourceDTO, ClassResource>, NewClassResourceEFMapper>();
        s.RegisterEFProjector<ClassResourceProjector, ClassResourceDomain, ClassResource>();
        // Minimal API
        s.AddSingleton<IMapper<NewClassResourceMAPI, Executor, NewClassResourceDTO>, NewClassResourceMAPIMapper>();
        s.AddSingleton<IMapper<Guid, string, Executor, DeleteClassResourceDTO>, DeleteClassResourceMAPIMapper>();

        return s;
    }
}
