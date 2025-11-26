using Application.DTOs.ClassProfessors;
using Application.DTOs.Common;
using Domain.Entities;
using Domain.Enums;
using EntityFramework.Application.DAOs.ClassProfessors;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Classes;
using EntityFramework.InterfaceAdapters.Mappers.ClassProfessors;
using EntityFramework.InterfaceAdapters.Mappers.Users;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkTest;

public class ClassProfessorsEFRepositoryTest : IDisposable
{
    private readonly EduZasDotnetContext _ctx;
    private readonly SqliteConnection _conn;
    private readonly ClassProfessorsEFCreator _creator;
    private readonly ClassProfessorsEFReader _reader;
    private readonly ClassProfessorsEFUpdater _updater;
    private readonly ClassProfessorsEFDeleter _deleter;

    private readonly UserProjector _userMapper = new();
    private readonly ClassProjector _classMapper = new();

    private readonly Random _rdm = new();

    public ClassProfessorsEFRepositoryTest()
    {
        var dbName = Guid.NewGuid().ToString();
        _conn = new SqliteConnection($"Data Source={dbName};Mode=Memory;Cache=Shared");
        _conn.Open();

        var opts = new DbContextOptionsBuilder<EduZasDotnetContext>().UseSqlite(_conn).Options;

        _ctx = new EduZasDotnetContext(opts);
        _ctx.Database.EnsureCreated();

        _classMapper = new();

        var mapper = new ClassProfessorProjector();

        _creator = new(_ctx, mapper, new NewClassProfessorEFMapper());
        _reader = new(_ctx, mapper);
        _updater = new(_ctx, mapper, new UpdateClassProfessorEFMapper());
        _deleter = new(_ctx, mapper);
    }

    private async Task<UserDomain> SeedUser(UserType role = UserType.PROFESSOR)
    {
        var id = (ulong)_rdm.NextInt64(1, 100_000);
        var user = new User
        {
            UserId = id,
            Email = $"test{id}@test.com",
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

    private async Task<ClassDomain> SeedClass()
    {
        var id = (ulong)_rdm.NextInt64(1, 100_000);
        var classEntity = new Class { ClassId = $"test-class-{id}", ClassName = "Test Class" };
        _ctx.Classes.Add(classEntity);
        await _ctx.SaveChangesAsync();
        return _classMapper.Map(classEntity);
    }

    [Fact]
    public async Task Add_ValidRelation_ReturnsRelation()
    {
        var admin = await SeedUser(UserType.ADMIN);
        var user = await SeedUser();
        var cls = await SeedClass();

        var newRelation = new NewClassProfessorDTO()
        {
            ClassId = cls.Id,
            UserId = user.Id,
            IsOwner = true,
            Executor = AsExecutor(admin),
        };

        var created = await _creator.AddAsync(newRelation);

        Assert.NotNull(created);
        Assert.Equal(newRelation.ClassId, created.Id.ClassId);
        Assert.Equal(newRelation.UserId, created.Id.UserId);
        Assert.Equal(newRelation.IsOwner, created.IsOwner);
    }

    [Fact]
    public async Task Add_DuplicateRelation_ThrowsException()
    {
        var admin = await SeedUser(UserType.ADMIN);
        var user = await SeedUser();
        var cls = await SeedClass();

        var newRelation = new NewClassProfessorDTO
        {
            ClassId = cls.Id,
            UserId = user.Id,
            IsOwner = true,
            Executor = AsExecutor(admin),
        };

        await _creator.AddAsync(newRelation);

        await Assert.ThrowsAnyAsync<Exception>(() => _creator.AddAsync(newRelation));
    }

    [Fact]
    public async Task Add_NonExistentClass_ThrowsException()
    {
        var admin = await SeedUser(UserType.ADMIN);
        var user = await SeedUser();

        var newRelation = new NewClassProfessorDTO
        {
            ClassId = "non-existent-class",
            UserId = user.Id,
            IsOwner = true,
            Executor = AsExecutor(admin),
        };

        await Assert.ThrowsAnyAsync<Exception>(() => _creator.AddAsync(newRelation));
    }

    [Fact]
    public async Task Get_ExistingRelation_ReturnsRelation()
    {
        var admin = await SeedUser(UserType.ADMIN);
        var user = await SeedUser();
        var cls = await SeedClass();

        var newRelation = new NewClassProfessorDTO
        {
            ClassId = cls.Id,
            UserId = user.Id,
            IsOwner = true,
            Executor = AsExecutor(admin),
        };

        var created = await _creator.AddAsync(newRelation);

        var found = await _reader.GetAsync(created.Id);

        Assert.True(found.IsSome);
        Assert.Equal(created.Id, found.Unwrap().Id);
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
        var admin = await SeedUser(UserType.ADMIN);
        var user = await SeedUser();
        var cls = await SeedClass();

        var newRelationDto = new NewClassProfessorDTO
        {
            ClassId = cls.Id,
            UserId = user.Id,
            IsOwner = false,
            Executor = AsExecutor(admin),
        };
        var created = await _creator.AddAsync(newRelationDto);

        var updateDto = new ClassProfessorUpdateDTO
        {
            UserId = created.Id.UserId,
            ClassId = created.Id.ClassId,
            IsOwner = true,
            Executor = AsExecutor(admin),
        };

        var updated = await _updater.UpdateAsync(updateDto);

        Assert.NotNull(updated);
        Assert.Equal(
            new UserClassRelationId { ClassId = updateDto.ClassId, UserId = updateDto.UserId },
            updated.Id
        );
        Assert.Equal(updateDto.IsOwner, updated.IsOwner);
    }

    [Fact]
    public async Task Update_NonExistingRelation_ThrowsException()
    {
        var admin = await SeedUser(UserType.ADMIN);
        var updateDto = new ClassProfessorUpdateDTO
        {
            ClassId = "non-existent",
            UserId = 99,
            IsOwner = true,
            Executor = AsExecutor(admin),
        };

        await Assert.ThrowsAnyAsync<Exception>(() => _updater.UpdateAsync(updateDto));
    }

    [Fact]
    public async Task Delete_ExistingRelation_ReturnsDeletedRelation()
    {
        var admin = await SeedUser(UserType.ADMIN);
        var user = await SeedUser();
        var cls = await SeedClass();

        var newRelationDto = new NewClassProfessorDTO
        {
            ClassId = cls.Id,
            UserId = user.Id,
            IsOwner = false,
            Executor = AsExecutor(admin),
        };
        var created = await _creator.AddAsync(newRelationDto);

        var deleted = await _deleter.DeleteAsync(created.Id);

        Assert.NotNull(deleted);
        Assert.Equal(created.Id, deleted.Id);

        var found = await _reader.GetAsync(created.Id);
        Assert.True(found.IsNone);
    }

    [Fact]
    public async Task Delete_NonExistingRelation_ThrowsException()
    {
        var admin = await SeedUser(UserType.ADMIN);
        var id = new UserClassRelationId { ClassId = "non-existent", UserId = 99 };

        await Assert.ThrowsAnyAsync<Exception>(() => _deleter.DeleteAsync(id));
    }

    public void Dispose()
    {
        _conn.Close();
        _conn.Dispose();
        _ctx.Dispose();
    }
}
