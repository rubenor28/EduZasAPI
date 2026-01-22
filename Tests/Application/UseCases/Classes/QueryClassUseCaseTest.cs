using Application.DTOs.Classes;
using Application.UseCases.Classes;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;
using EntityFramework.Application.DAOs.Classes;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Classes;
using EntityFramework.InterfaceAdapters.Mappers.Users;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Tests.Application.UseCases.Classes;

public class QueryClassUseCaseTest : IDisposable
{
    private readonly QueryClassUseCase _useCase;
    private readonly EduZasDotnetContext _ctx;
    private readonly SqliteConnection _conn;
    private readonly ClassProjector _classMapper = new();
    private readonly UserMapper _userMapper = new();
    private readonly Random _rdm = new();

    public QueryClassUseCaseTest()
    {
        var dbName = Guid.NewGuid().ToString();
        _conn = new SqliteConnection($"Data Source={dbName};Mode=Memory;Cache=Shared");
        _conn.Open();

        var opts = new DbContextOptionsBuilder<EduZasDotnetContext>().UseSqlite(_conn).Options;
        _ctx = new EduZasDotnetContext(opts);
        _ctx.Database.EnsureCreated();

        var querier = new ClassEFQuerier(_ctx, _classMapper, maxPageSize: 10);

        _useCase = new QueryClassUseCase(querier, null);
    }

    private async Task<UserDomain> SeedUser(UserType role = UserType.STUDENT)
    {
        var id = (ulong)_rdm.NextInt64(1, 1_000_000);
        var user = new User
        {
            UserId = id,
            Email = $"user-{id}@test.com",
            FirstName = "test",
            FatherLastname = "test",
            Password = "test",
            Role = (uint)role,
        };

        _ctx.Users.Add(user);
        await _ctx.SaveChangesAsync();
        return _userMapper.Map(user);
    }

    private static Executor AsExecutor(UserDomain u) => new() { Id = u.Id, Role = u.Role };

    private async Task<Class> SeedClass(string name)
    {
        var id = (ulong)_rdm.NextInt64(1, 1_000_000);
        var cls = new Class
        {
            ClassId = $"class-{id}",
            ClassName = name,
            Active = true,
        };
        _ctx.Classes.Add(cls);
        await _ctx.SaveChangesAsync();
        return cls;
    }

    private async Task SeedClassProfessor(string classId, ulong professorId, bool isOwner)
    {
        var relation = new ClassProfessor
        {
            ClassId = classId,
            ProfessorId = professorId,
            IsOwner = isOwner,
        };
        _ctx.ClassProfessors.Add(relation);
        await _ctx.SaveChangesAsync();
    }

    private async Task SeedClassStudent(string classId, ulong studentId)
    {
        var relation = new ClassStudent
        {
            ClassId = classId,
            StudentId = studentId,
            Hidden = false,
        };
        _ctx.ClassStudents.Add(relation);
        await _ctx.SaveChangesAsync();
    }

    [Fact]
    public async Task ExecuteAsync_WithNoCriteria_ReturnsAllClasses()
    {
        var admin = await SeedUser(UserType.ADMIN);
        await SeedClass("Class A");
        await SeedClass("Class B");
        await SeedClass("Class C");

        var result = await _useCase.ExecuteAsync(
            new() { Data = new ClassCriteriaDTO(), Executor = AsExecutor(admin) }
        );

        Assert.True(result.IsOk);
        Assert.Equal(3, result.Unwrap().Results.Count());
    }

    [Fact]
    public async Task ExecuteAsync_FilterByClassName_ReturnsMatchingClass()
    {
        var admin = await SeedUser(UserType.ADMIN);
        await SeedClass("Class A");
        var classB = await SeedClass("Class B");
        await SeedClass("Class C");

        var criteria = new ClassCriteriaDTO
        {
            ClassName = new StringQueryDTO { Text = "Class B", SearchType = StringSearchType.EQ },
        };

        var result = await _useCase.ExecuteAsync(
            new() { Data = criteria, Executor = AsExecutor(admin) }
        );

        Assert.True(result.IsOk);
        var classes = result.Unwrap().Results.ToList();
        Assert.Single(classes);
        Assert.Equal(classB.ClassId, classes.First().Id);
    }

    [Fact]
    public async Task ExecuteAsync_FilterWithProfessor_ReturnsMatchingClasses()
    {
        var admin = await SeedUser(UserType.ADMIN);
        var profA = await SeedUser(UserType.PROFESSOR);
        var profB = await SeedUser(UserType.PROFESSOR);
        var classA = await SeedClass("Class A");
        var classB = await SeedClass("Class B");
        await SeedClassProfessor(classA.ClassId, profA.Id, true);
        await SeedClassProfessor(classB.ClassId, profB.Id, true);

        var criteria = new ClassCriteriaDTO
        {
            WithProfessor = new WithProfessorDTO { Id = profA.Id },
        };

        var result = await _useCase.ExecuteAsync(
            new() { Data = criteria, Executor = AsExecutor(admin) }
        );

        Assert.True(result.IsOk);
        var classes = result.Unwrap().Results.ToList();
        Assert.Single(classes);
        Assert.Equal(classA.ClassId, classes.First().Id);
    }

    [Fact]
    public async Task ExecuteAsync_FilterWithStudent_ReturnsMatchingClasses()
    {
        var admin = await SeedUser(UserType.ADMIN);
        var studentA = await SeedUser();
        var studentB = await SeedUser();
        var classA = await SeedClass("Class A");
        var classB = await SeedClass("Class B");
        await SeedClassStudent(classA.ClassId, studentA.Id);
        await SeedClassStudent(classB.ClassId, studentB.Id);

        var criteria = new ClassCriteriaDTO
        {
            WithStudent = new WithStudentDTO { Id = studentB.Id },
        };

        var result = await _useCase.ExecuteAsync(
            new() { Data = criteria, Executor = AsExecutor(admin) }
        );

        Assert.True(result.IsOk);
        var classes = result.Unwrap().Results.ToList();
        Assert.Single(classes);
        Assert.Equal(classB.ClassId, classes.First().Id);
    }

    [Fact]
    public async Task ExecuteAsync_WithNoMatches_ReturnsEmptyList()
    {
        var admin = await SeedUser(UserType.ADMIN);
        await SeedClass("Class A");

        var criteria = new ClassCriteriaDTO
        {
            ClassName = new StringQueryDTO
            {
                Text = "Non Existent",
                SearchType = StringSearchType.EQ,
            },
        };

        var result = await _useCase.ExecuteAsync(
            new() { Data = criteria, Executor = AsExecutor(admin) }
        );

        Assert.True(result.IsOk);
        Assert.Empty(result.Unwrap().Results);
    }

    public void Dispose()
    {
        _conn.Close();
        _conn.Dispose();
        _ctx.Dispose();
    }
}
