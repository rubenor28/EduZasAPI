using Application.DTOs.Classes;
using Application.DTOs.ClassProfessors;
using Application.DTOs.ClassStudents;
using Application.DTOs.Common;
using Application.DTOs.Contacts;
using Application.DTOs.ContactTags;
using Application.DTOs.Notifications;
using Application.DTOs.Tags;
using Application.DTOs.Tests;
using Application.DTOs.UserNotifications;
using Application.DTOs.Users;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;
using EntityFramework.Application.DAOs.Classes;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers;
using InterfaceAdapters.Mappers.Common;
using InterfaceAdapters.Mappers.Users;
using MinimalAPI.Application.DTOs.Classes;
using MinimalAPI.Application.DTOs.ClassProfessors;
using MinimalAPI.Application.DTOs.ClassStudents;
using MinimalAPI.Application.DTOs.Common;
using MinimalAPI.Application.DTOs.Users;
using MinimalAPI.Presentation.Mappers;

namespace MinimalAPI.Extensions;

/// <summary>
/// Métodos de extensión para registrar los servicios de base de datos en el contenedor de dependencias.
/// </summary>
public static class MapperServiceCollectionExtensions
{
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
        s.AddSingleton<StringSearchMapper>();

        s.AddSingleton<IMapper<string, Result<StringSearchType, Unit>>>(sp =>
            sp.GetRequiredService<StringSearchMapper>());

        s.AddSingleton<IMapper<StringSearchType, string>>(sp =>
            sp.GetRequiredService<StringSearchMapper>());

        s.AddSingleton<StringQueryMAPIMapper>();

        s.AddSingleton<IMapper<Optional<StringQueryDTO>, StringQueryMAPI?>>(sp =>
            sp.GetRequiredService<StringQueryMAPIMapper>());

        s.AddSingleton<IMapper<StringQueryMAPI?, Result<Optional<StringQueryDTO>, Unit>>>(sp =>
            sp.GetRequiredService<StringQueryMAPIMapper>());

        s.AddSingleton<IMapper<StringQueryMAPI?, Result<Optional<StringQueryDTO>, Unit>>>(sp =>
            sp.GetRequiredService<StringQueryMAPIMapper>());

        // USERS
        // EF
        s.AddSingleton<UserTypeMapper>();

        s.AddSingleton<IMapper<string, Result<UserType, Unit>>>(sp =>
            sp.GetRequiredService<UserTypeMapper>());

        s.AddSingleton<IMapper<int, Result<UserType, Unit>>>(sp =>
            sp.GetRequiredService<UserTypeMapper>());

        s.AddSingleton<IMapper<uint, Result<UserType, Unit>>>(sp =>
            sp.GetRequiredService<UserTypeMapper>());

        s.AddSingleton<IMapper<UserType, uint>>(sp => sp.GetRequiredService<UserTypeMapper>());

        s.AddSingleton<IMapper<UserType, ulong>>(sp => sp.GetRequiredService<UserTypeMapper>());

        s.AddSingleton<UserEFMapper>();

        s.AddSingleton<IMapper<User, UserDomain>>(sp => sp.GetRequiredService<UserEFMapper>());

        s.AddSingleton<IMapper<NewUserDTO, User>>(sp => sp.GetRequiredService<UserEFMapper>());

        s.AddSingleton<IUpdateMapper<UserUpdateDTO, User>>(sp =>
            sp.GetRequiredService<UserEFMapper>());

        // Minimal API
        s.AddSingleton<UserMAPIMapper>();

        s.AddSingleton<IMapper<UserCriteriaMAPI, Result<UserCriteriaDTO, IEnumerable<FieldErrorDTO>>>>(sp => 
            sp.GetRequiredService<UserMAPIMapper>());

        s.AddSingleton<IMapper<NewUserMAPI, Executor, NewUserDTO>>(sp =>
            sp.GetRequiredService<UserMAPIMapper>());

        s.AddSingleton<IMapper<UserDomain, PublicUserMAPI>>(sp =>
            sp.GetRequiredService<UserMAPIMapper>());

        // CLASSES
        // EF
        s.AddSingleton<ClassEFMapper>();

        s.AddSingleton<IMapper<NewClassDTO, Class>>(sp => sp.GetRequiredService<ClassEFMapper>());

        s.AddSingleton<IMapper<Class, ClassDomain>>(sp => sp.GetRequiredService<ClassEFMapper>());

        s.AddSingleton<IUpdateMapper<ClassUpdateDTO, Class>>(sp =>
            sp.GetRequiredService<ClassEFMapper>());

        //Minimal API
        s.AddSingleton<ClassMAPIMapper>();

        s.AddSingleton<IMapper<ClassDomain, PublicClassMAPI>>(sp =>
            sp.GetRequiredService<ClassMAPIMapper>());

        s.AddSingleton<IMapper<NewClassMAPI, Executor, NewClassDTO>>(sp =>
            sp.GetRequiredService<ClassMAPIMapper>());

        s.AddSingleton<IMapper<ClassUpdateMAPI, Executor, ClassUpdateDTO>>(sp =>
            sp.GetRequiredService<ClassMAPIMapper>());

        s.AddSingleton<IMapper<string, Executor, DeleteClassDTO>>(sp =>
            sp.GetRequiredService<ClassMAPIMapper>());

        s.AddSingleton<IMapper<ClassCriteriaMAPI, Result<ClassCriteriaDTO, IEnumerable<FieldErrorDTO>>>>(sp =>
            sp.GetRequiredService<ClassMAPIMapper>());

        s.AddSingleton<IMapper<PaginatedQuery<ClassDomain, ClassCriteriaDTO>,PaginatedQuery<PublicClassMAPI, ClassCriteriaMAPI>>>(sp =>
            sp.GetRequiredService<ClassMAPIMapper>());

        // CLASS PROFESSOR
        // EF
        s.AddSingleton<ProfessorClassEFMapper>();

        s.AddSingleton<IMapper<ProfessorClassRelationDTO, ClassProfessor>>(sp =>
            sp.GetRequiredService<ProfessorClassEFMapper>());

        s.AddSingleton<IMapper<ClassProfessor, ProfessorClassRelationDTO>>(sp =>
            sp.GetRequiredService<ProfessorClassEFMapper>());

        s.AddSingleton<IUpdateMapper<ProfessorClassRelationDTO, ClassProfessor>>(sp =>
            sp.GetRequiredService<ProfessorClassEFMapper>());

        // Minimal API
        s.AddSingleton<ClassProfessorMAPIMapper>();
        s.AddSingleton<IMapper<NewClassProfessorMAPI, Executor, NewClassProfessorDTO>>(sp =>
            sp.GetRequiredService<ClassProfessorMAPIMapper>());

        // CLASS STUDENTS
        s.AddSingleton<StudentClassEFMapper>();

        s.AddSingleton<IMapper<EnrollClassDTO, ClassStudent>>(sp =>
            sp.GetRequiredService<StudentClassEFMapper>());

        s.AddSingleton<IMapper<ClassStudent, StudentClassRelationDTO>>(sp =>
            sp.GetRequiredService<StudentClassEFMapper>());

        s.AddSingleton<IUpdateMapper<StudentClassRelationDTO, ClassStudent>>(sp =>
            sp.GetRequiredService<StudentClassEFMapper>());

        s.AddSingleton<IMapper<EnrollClassMAPI, Executor, EnrollClassDTO>>(sp =>
            sp.GetRequiredService<ClassStudentsMAPIMapper>());

        s.AddSingleton<IMapper<string, ulong, Executor, UnenrollClassDTO>>(sp =>
            sp.GetRequiredService<ClassStudentsMAPIMapper>());

        s.AddSingleton<IMapper<string, ulong, Executor, ToggleClassVisibilityDTO>>(sp =>
            sp.GetRequiredService<ClassStudentsMAPIMapper>());


        // NOTIFICATIONS
        s.AddSingleton<NotificationEFMapper>();

        s.AddSingleton<IMapper<Notification, NotificationDomain>>(sp =>
            sp.GetRequiredService<NotificationEFMapper>());

        s.AddSingleton<IMapper<NewNotificationDTO, Notification>>(sp =>
            sp.GetRequiredService<NotificationEFMapper>());

        // USER NOTIFICATIONS
        s.AddSingleton<UserNotificationEFMapper>();

        s.AddSingleton<IMapper<NotificationPerUser, UserNotificationDomain>>(sp =>
            sp.GetRequiredService<UserNotificationEFMapper>());

        s.AddSingleton<IMapper<NewUserNotificationDTO, NotificationPerUser>>(sp =>
            sp.GetRequiredService<UserNotificationEFMapper>());

        s.AddSingleton<IUpdateMapper<UserNotificationUpdateDTO, NotificationPerUser>>(sp =>
            sp.GetRequiredService<UserNotificationEFMapper>());

        // TAGS
        s.AddSingleton<TagEFMapper>();

        s.AddSingleton<IMapper<Tag, TagDomain>>(sp => sp.GetRequiredService<TagEFMapper>());

        s.AddSingleton<IMapper<NewTagDTO, Tag>>(sp => sp.GetRequiredService<TagEFMapper>());

        // CONTACTS
        s.AddSingleton<ContactEFMapper>();

        s.AddSingleton<IMapper<AgendaContact, ContactDomain>>(sp =>
            sp.GetRequiredService<ContactEFMapper>());

        s.AddSingleton<IMapper<NewContactDTO, AgendaContact>>(sp =>
            sp.GetRequiredService<ContactEFMapper>());

        s.AddSingleton<IUpdateMapper<ContactUpdateDTO, AgendaContact>>(sp =>
            sp.GetRequiredService<ContactEFMapper>());

        // CONTACT TAGS
        s.AddSingleton<ContactTagEFMapper>();

        s.AddSingleton<IMapper<ContactTag, ContactTagDomain>>(sp =>
            sp.GetRequiredService<ContactTagEFMapper>());

        s.AddSingleton<IMapper<NewContactTagDTO, ContactTag>>(sp =>
            sp.GetRequiredService<ContactTagEFMapper>());

        // TESTS
        s.AddSingleton<TestEFMapper>();

        s.AddSingleton<IMapper<Test, TestDomain>>(sp => sp.GetRequiredService<TestEFMapper>());

        s.AddSingleton<IMapper<NewTestDTO, Test>>(sp => sp.GetRequiredService<TestEFMapper>());

        s.AddSingleton<IUpdateMapper<TestUpdateDTO, Test>>(sp =>
            sp.GetRequiredService<TestEFMapper>());

        return s;
    }
}
