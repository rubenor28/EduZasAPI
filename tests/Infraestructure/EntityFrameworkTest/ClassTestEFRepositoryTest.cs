using Application.DTOs.Classes;
using Application.DTOs.ClassTests;
using Application.DTOs.Common;
using Application.DTOs.Tests;
using Application.DTOs.Users;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;
using EntityFramework.Application.DAOs.Classes;
using EntityFramework.Application.DAOs.ClassTests;
using EntityFramework.Application.DAOs.Tests;
using EntityFramework.Application.DAOs.Users;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers;
using InterfaceAdapters.Mappers.Users;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkTest;

public class ClassTestEFRepositoryTest : IDisposable
{
    private readonly EduZasDotnetContext _ctx;
    private readonly SqliteConnection _conn;

    private readonly ClassTestEFCreator _creator;
    private readonly ClassTestEFUpdater _updater;
    private readonly ClassTestEFDeleter _deleter;
    private readonly ClassTestEFReader _reader;

    private readonly ClassEFCreator _classCreator;
    private readonly TestEFCreator _testCreator;
    private readonly UserEFCreator _userCreator;

    public ClassTestEFRepositoryTest()
    {
        var dbName = Guid.NewGuid().ToString();
        _conn = new SqliteConnection($"Data Source={dbName};Mode=Memory;Cache=Shared");
        _conn.Open();

        var opts = new DbContextOptionsBuilder<EduZasDotnetContext>().UseSqlite(_conn).Options;

        _ctx = new EduZasDotnetContext(opts);
        _ctx.Database.EnsureCreated();

        var classTestMapper = new ClassTestEFMapper();
        var userTypeMapper = new UserTypeMapper();
        var classMapper = new ClassEFMapper();
        var testMapper = new TestEFMapper();
        var userMapper = new UserEFMapper(userTypeMapper, userTypeMapper);

        _creator = new(_ctx, classTestMapper, classTestMapper);
        _updater = new(_ctx, classTestMapper, classTestMapper);
        _deleter = new(_ctx, classTestMapper);
        _reader = new(_ctx, classTestMapper);

        _classCreator = new(_ctx, classMapper, classMapper);
        _testCreator = new(_ctx, testMapper, testMapper);
        _userCreator = new(_ctx, userMapper, userMapper);
    }

    private async Task<UserDomain> CreateSampleProfessor()
    {
        var newUser = new NewUserDTO
        {
            FirstName = "Test",
            FatherLastName = "Professor",
            Email = "test.professor@email.com",
            Password = "password",
        };
        return await _userCreator.AddAsync(newUser);
    }

    private static Executor AsExecutor(UserDomain user) => new() { Id = user.Id, Role = user.Role };

    private async Task<ClassDomain> CreateSampleClass(UserDomain professor)
    {
        var newClass = new NewClassDTO
        {
            Id = "test-class",
            ClassName = "Test Class",
            Color = "#ffffff",
            Section = Optional.Some("A"),
            Subject = Optional.Some("Math"),
            OwnerId = professor.Id,
            Executor = AsExecutor(professor),
        };

        return await _classCreator.AddAsync(newClass);
    }

    private async Task<TestDomain> CreateSampleTest(UserDomain professor)
    {
        var newTest = new NewTestDTO
        {
            Title = "Test Title",
            Content = "Test Content",
            ProfessorId = professor.Id,
            Executor = new() { Id = professor.Id, Role = professor.Role },
        };
        return await _testCreator.AddAsync(newTest);
    }

    [Fact]
    public async Task AddClassTest_ReturnsClassTest()
    {
        var professor = await CreateSampleProfessor();
        var createdClass = await CreateSampleClass(professor);
        var createdTest = await CreateSampleTest(professor);

        var newClassTest = new NewClassTestDTO
        {
            ClassId = createdClass.Id,
            TestId = createdTest.Id,
            Visible = true,
            Executor = AsExecutor(professor),
        };

        var created = await _creator.AddAsync(newClassTest);

        Assert.NotNull(created);
        Assert.Equal(newClassTest.ClassId, created.Id.ClassId);
        Assert.Equal(newClassTest.TestId, created.Id.TestId);
    }

    [Fact]
    public async Task UpdateClassTest_ReturnsUpdatedClassTest()
    {
        var professor = await CreateSampleProfessor();
        var createdClass = await CreateSampleClass(professor);
        var createdTest = await CreateSampleTest(professor);

        var newClassTestDto = new NewClassTestDTO
        {
            ClassId = createdClass.Id,
            TestId = createdTest.Id,
            Visible = true,
            Executor = AsExecutor(professor),
        };
        await _creator.AddAsync(newClassTestDto);

        var updateDto = new ClassTestUpdateDTO
        {
            Id = new() { ClassId = createdClass.Id, TestId = createdTest.Id },
            Visible = false,
            Executor = AsExecutor(professor),
        };

        var updatedClassTest = await _updater.UpdateAsync(updateDto);

        Assert.NotNull(updatedClassTest);
        Assert.Equal(updateDto.Visible, updatedClassTest.Visible);
    }

    [Fact]
    public async Task DeleteClassTest_ShouldRemoveRelation()
    {
        var professor = await CreateSampleProfessor();
        var createdClass = await CreateSampleClass(professor);
        var createdTest = await CreateSampleTest(professor);

        var newClassTestDto = new NewClassTestDTO
        {
            ClassId = createdClass.Id,
            TestId = createdTest.Id,
            Visible = true,
            Executor = AsExecutor(professor),
        };

        await _creator.AddAsync(newClassTestDto);

        var idDto = new ClassTestIdDTO { ClassId = createdClass.Id, TestId = createdTest.Id };

        var deleted = await _deleter.DeleteAsync(idDto);

        Assert.NotNull(deleted);

        var found = await _reader.GetAsync(idDto);
        Assert.True(found.IsNone);
    }

    public void Dispose()
    {
        _conn.Close();
        _conn.Dispose();
        _ctx.Dispose();
    }
}
