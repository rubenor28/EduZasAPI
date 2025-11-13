using Application.DTOs.Classes;
using Application.DTOs.ClassStudents;
using Application.DTOs.Common;
using Domain.Entities;
using Domain.Enums;
using EntityFramework.Application.DAOs.ClassStudents;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers;
using InterfaceAdapters.Mappers.Users;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkTest;

public class StudentPerClassEFRepositoryTest : IDisposable
{
    private readonly EduZasDotnetContext _ctx;
    private readonly SqliteConnection _conn;

    private readonly ClassStudentEFCreator _creator;
    private readonly ClassStudentsEFReader _reader;
    private readonly ClassStudentsEFUpdater _updater;
    private readonly ClassStudentsEFDeleter _deleter;

    private readonly ClassEFMapper _classMapper = new();
    private readonly UserEFMapper _userMapper;

    public StudentPerClassEFRepositoryTest()
    {
        var dbName = Guid.NewGuid().ToString();
        _conn = new SqliteConnection($"Data Source={dbName};Mode=Memory;Cache=Shared");
        _conn.Open();

        var opts = new DbContextOptionsBuilder<EduZasDotnetContext>().UseSqlite(_conn).Options;

        _ctx = new EduZasDotnetContext(opts);
        _ctx.Database.EnsureCreated();

        var mapper = new ClassStudentEFMapper();

        var roleMapper = new UserTypeMapper();
        _userMapper = new(roleMapper, roleMapper);

        _creator = new(_ctx, mapper, mapper);
        _reader = new(_ctx, mapper);
        _updater = new(_ctx, mapper, mapper);
        _deleter = new(_ctx, mapper);
    }

    private async Task<UserDomain> SeedUser()
    {
        var user = new User
        {
            UserId = 1,
            Email = "test@test.com",
            FirstName = "test",
            FatherLastname = "test",
            Password = "test",
        };

        _ctx.Users.Add(user);
        await _ctx.SaveChangesAsync();
        return _userMapper.Map(user);
    }

    private static Executor AsExecutor(UserDomain user) => new() { Id = user.Id, Role = user.Role };

    private async Task<ClassDomain> SeedClass()
    {
        var classEntity = new Class { ClassId = "test-class", ClassName = "Test Class" };
        _ctx.Classes.Add(classEntity);
        await _ctx.SaveChangesAsync();
        return _classMapper.Map(classEntity);
    }

    [Fact]
    public async Task Add_ValidRelation_ReturnsRelation()
    {
        var user = await SeedUser();
        var cls = await SeedClass();
        var newRelation = new NewClassStudentDTO
        {
            ClassId = cls.Id,
            UserId = user.Id,
            Executor = AsExecutor(user),
        };

        var created = await _creator.AddAsync(newRelation);

        Assert.NotNull(created);
        Assert.Equal(newRelation.ClassId, created.Id.ClassId);
        Assert.Equal(newRelation.UserId, created.Id.UserId);
    }

    [Fact]
    public async Task Add_DuplicateRelation_ThrowsException()
    {
        var user = await SeedUser();
        var cls = await SeedClass();

        var newRelation = new NewClassStudentDTO
        {
            ClassId = cls.Id,
            UserId = user.Id,
            Executor = AsExecutor(user),
        };

        await _creator.AddAsync(newRelation);
        await Assert.ThrowsAnyAsync<Exception>(() => _creator.AddAsync(newRelation));
    }

    [Fact]
    public async Task Get_ExistingRelation_ReturnsRelation()
    {
        var user = await SeedUser();
        var cls = await SeedClass();

        var newRelation = new NewClassStudentDTO
        {
            ClassId = cls.Id,
            UserId = user.Id,
            Executor = AsExecutor(user),
        };

        var created = await _creator.AddAsync(newRelation);

        var found = await _reader.GetAsync(created.Id);

        Assert.True(found.IsSome);
        Assert.Equal(newRelation.UserId, found.Unwrap().Id.UserId);
        Assert.Equal(newRelation.ClassId, found.Unwrap().Id.ClassId);
    }

    [Fact]
    public async Task Get_NonExistingRelation_ReturnsEmptyOptional()
    {
        var found = await _reader.GetAsync(new() { ClassId = "non-existent", UserId = 99 });

        Assert.True(found.IsNone);
    }

    [Fact]
    public async Task Update_ExistingRelation_ReturnsUpdatedRelation()
    {
        var user = await SeedUser();
        var cls = await SeedClass();

        var newRelation = new NewClassStudentDTO
        {
            ClassId = cls.Id,
            UserId = user.Id,
            Executor = AsExecutor(user),
        };

        var created = await _creator.AddAsync(newRelation);

        var toUpdate = new ClassStudentUpdateDTO
        {
            Id = created.Id,
            Hidden = true,
            Executor = AsExecutor(user),
        };

        var updated = await _updater.UpdateAsync(toUpdate);

        Assert.NotNull(updated);
        Assert.True(updated.Hidden);
    }

    [Fact]
    public async Task Update_NonExistingRelation_ThrowsException()
    {
        var toUpdate = new ClassStudentUpdateDTO
        {
            Id = new() { ClassId = "non-existent", UserId = 99 },
            Hidden = true,
            Executor = new() { Id = 1, Role = UserType.ADMIN },
        };

        await Assert.ThrowsAnyAsync<Exception>(() => _updater.UpdateAsync(toUpdate));
    }

    [Fact]
    public async Task Delete_ExistingRelation_ReturnsDeletedRelation()
    {
        var user = await SeedUser();
        var cls = await SeedClass();

        var newRelation = new NewClassStudentDTO
        {
            ClassId = cls.Id,
            UserId = user.Id,
            Executor = AsExecutor(user),
        };

        var created = await _creator.AddAsync(newRelation);

        var deleted = await _deleter.DeleteAsync(created.Id);

        Assert.NotNull(deleted);
        Assert.Equal(created.Id, deleted.Id);

        var found = await _reader.GetAsync(created.Id);
        Assert.True(found.IsNone);
    }

    [Fact]
    public async Task Delete_NonExistingRelation_ThrowsException()
    {
        await Assert.ThrowsAnyAsync<Exception>(() =>
            _deleter.DeleteAsync(new() { ClassId = "non-existent", UserId = 99 })
        );
    }

    public void Dispose()
    {
        _conn.Close();
        _conn.Dispose();
        _ctx.Dispose();
    }
}
