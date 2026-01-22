using Application.DAOs;
using Application.DTOs.Classes;
using Application.DTOs.ClassProfessors;
using Application.DTOs.ClassTests;
using Domain.ValueObjects;
using Application.DTOs.Contacts;
using Application.DTOs.ContactTags;
using Application.DTOs.Notifications;
using Application.DTOs.Tags;
using Application.DTOs.Tests;
using Application.DTOs.Users;
using Domain.Entities;
using Domain.Entities.Questions;
using Domain.Enums;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;

namespace Tests;

public abstract class BaseTest : IDisposable
{
    protected readonly ServiceProvider _sp;
    protected readonly SqliteConnection _conn;
    protected readonly Random _random = new();

    // Creators
    protected readonly ICreatorAsync<UserDomain, NewUserDTO> _userCreator;
    protected readonly ICreatorAsync<ClassDomain, NewClassDTO> _classCreator;
    protected readonly ICreatorAsync<ClassProfessorDomain, NewClassProfessorDTO> _classProfessorCreator;
    protected readonly ICreatorAsync<ClassStudentDomain, UserClassRelationId> _classStudentCreator;
    protected readonly ICreatorAsync<TestDomain, NewTestDTO> _testCreator;
    protected readonly ICreatorAsync<ClassTestDomain, ClassTestIdDTO> _classTestCreator;
    protected readonly ICreatorAsync<ContactDomain, NewContactDTO> _contactCreator;
    protected readonly ICreatorAsync<TagDomain, NewTagDTO> _tagCreator;
    protected readonly ICreatorAsync<ContactTagDomain, NewContactTagDTO> _contactTagCreator;
    protected readonly ICreatorAsync<NotificationDomain, NewNotificationDTO> _notificationCreator;

    protected BaseTest()
    {
        _sp = ServiceProviderFactory.CreateServiceProvider();
        _conn = _sp.GetRequiredService<SqliteConnection>();

        _userCreator = _sp.GetRequiredService<ICreatorAsync<UserDomain, NewUserDTO>>();
        _classCreator = _sp.GetRequiredService<ICreatorAsync<ClassDomain, NewClassDTO>>();
        _classProfessorCreator = _sp.GetRequiredService<ICreatorAsync<ClassProfessorDomain, NewClassProfessorDTO>>();
        _classStudentCreator = _sp.GetRequiredService<ICreatorAsync<ClassStudentDomain, UserClassRelationId>>();
        _testCreator = _sp.GetRequiredService<ICreatorAsync<TestDomain, NewTestDTO>>();
        _classTestCreator = _sp.GetRequiredService<ICreatorAsync<ClassTestDomain, ClassTestIdDTO>>();
        _contactCreator = _sp.GetRequiredService<ICreatorAsync<ContactDomain, NewContactDTO>>();
        _tagCreator = _sp.GetRequiredService<ICreatorAsync<TagDomain, NewTagDTO>>();
        _contactTagCreator = _sp.GetRequiredService<ICreatorAsync<ContactTagDomain, NewContactTagDTO>>();
        _notificationCreator = _sp.GetRequiredService<ICreatorAsync<NotificationDomain, NewNotificationDTO>>();
    }

    protected static Executor AsExecutor(UserDomain user) =>
        new() { Id = user.Id, Role = user.Role };
        
    protected Task<NotificationDomain> SeedNotification(string classId, string title = "Test Notification") =>
        _notificationCreator.AddAsync(
            new()
            {
                ClassId = classId,
                Title = title,
            }
        );

    protected Task<UserDomain> SeedUser(
        UserType role = UserType.STUDENT,
        string? email = null,
        string password = "Password123!"
    ) =>
        _userCreator.AddAsync(
            new()
            {
                Email = email ?? $"{Guid.NewGuid().ToString()[10..]}@test.com",
                FirstName = "test",
                FatherLastname = "test",
                Password = password,
                Role = role,
            }
        );

    protected Task<ClassDomain> SeedClass(
        ulong ownerId,
        string? id = null,
        ICollection<Professor>? professors = null
    ) =>
        _classCreator.AddAsync(
            new()
            {
                Id = id ?? $"class-test-{_random.Next(0, 10000)}",
                ClassName = "Test Class",
                Color = "#fffff",
                OwnerId = ownerId,
                Professors = professors ?? [],
            }
        );

    protected Task<ClassProfessorDomain> SeedClassProfessor(string classId, ulong professorId, bool isOwner) =>
        _classProfessorCreator.AddAsync(
            new()
            {
                ClassId = classId,
                UserId = professorId,
                IsOwner = isOwner,
            }
        );

    protected Task<ClassStudentDomain> SeedClassStudent(string classId, ulong studentId) =>
        _classStudentCreator.AddAsync(
            new() { ClassId = classId, UserId = studentId, }
        );

    protected Task<TestDomain> SeedTest(
        ulong professorId,
        IDictionary<Guid, IQuestion>? content = null
    ) =>
        _testCreator.AddAsync(
            new()
            {
                Title = "Sample Test",
                Color = "#aabbcc",
                ProfessorId = professorId,
                Content = content ?? new Dictionary<Guid, IQuestion>()
            }
        );
    
    protected Task<ClassTestDomain> SeedClassTest(string classId, Guid testId) =>
        _classTestCreator.AddAsync(
            new()
            {
                ClassId = classId,
                TestId = testId,
            }
        );
    
    protected Task<ContactDomain> SeedContact(ulong ownerId, ulong userId) =>
        _contactCreator.AddAsync(
            new()
            {
                AgendaOwnerId = ownerId,
                UserId = userId,
                Alias = "Test Contact"
            }
        );

    protected Task<TagDomain> SeedTag(string text) =>
        _tagCreator.AddAsync(new() { Text = text });
    
    protected Task<ContactTagDomain> SeedContactTag(ulong ownerId, ulong userId, ulong tagId) =>
        _contactTagCreator.AddAsync(
            new()
            {
                AgendaOwnerId = ownerId,
                UserId = userId,
                TagId = tagId
            }
        );


    public void Dispose()
    {
        _conn.Close();
        _conn.Dispose();
    }
}
