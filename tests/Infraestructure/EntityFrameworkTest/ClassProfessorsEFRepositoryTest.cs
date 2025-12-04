using Application.DTOs.ClassProfessors;
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

    private readonly UserMapper _userMapper = new();
    private readonly ClassMapper _classMapper = new();

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

        var mapper = new ClassProfessorMapper();

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
        var user = await SeedUser();
        var cls = await SeedClass();

        var newRelation = new NewClassProfessorDTO()
        {
            ClassId = cls.Id,
            UserId = user.Id,
            IsOwner = true,
        };

        var created = await _creator.AddAsync(newRelation);

        Assert.NotNull(created);
        Assert.Equal(newRelation.ClassId, created.ClassId);
        Assert.Equal(newRelation.UserId, created.UserId);
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
        };

        await Assert.ThrowsAnyAsync<Exception>(() => _creator.AddAsync(newRelation));
    }

    [Fact]
    public async Task Get_ExistingRelation_ReturnsRelation()
    {
        var user = await SeedUser();
        var cls = await SeedClass();

        var newRelation = new NewClassProfessorDTO
        {
            ClassId = cls.Id,
            UserId = user.Id,
            IsOwner = true,
        };

        var created = await _creator.AddAsync(newRelation);

        var found = await _reader.GetAsync(
            new() { ClassId = created.ClassId, UserId = created.UserId }
        );

        Assert.NotNull(found);
        Assert.Equal(created.ClassId, found.ClassId);
        Assert.Equal(created.UserId, found.UserId);
    }

    [Fact]
    public async Task Get_NonExistingRelation_ReturnsEmptyOptional()
    {
        var found = await _reader.GetAsync(new() { ClassId = "non-existent", UserId = 99 });
        Assert.Null(found);
    }

    [Fact]
    public async Task Update_ExistingRelation_ReturnsUpdatedRelation()
    {
        var user = await SeedUser();
        var cls = await SeedClass();

        var newRelationDto = new NewClassProfessorDTO
        {
            ClassId = cls.Id,
            UserId = user.Id,
            IsOwner = false,
        };
        var created = await _creator.AddAsync(newRelationDto);

        var updateDto = new ClassProfessorUpdateDTO
        {
            UserId = created.UserId,
            ClassId = created.ClassId,
            IsOwner = true,
        };

        var updated = await _updater.UpdateAsync(updateDto);

        Assert.NotNull(updated);
        Assert.Equal(updateDto.ClassId, updated.ClassId);
        Assert.Equal(updateDto.UserId, updated.UserId);
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
        };

        await Assert.ThrowsAnyAsync<Exception>(() => _updater.UpdateAsync(updateDto));
    }

    [Fact]
    public async Task Delete_ExistingRelation_ReturnsDeletedRelation()
    {
        var user = await SeedUser();
        var cls = await SeedClass();

        var newRelationDto = new NewClassProfessorDTO
        {
            ClassId = cls.Id,
            UserId = user.Id,
            IsOwner = false,
        };

        var created = await _creator.AddAsync(newRelationDto);
        var id = new UserClassRelationId { ClassId = created.ClassId, UserId = created.UserId };

        var deleted = await _deleter.DeleteAsync(id);

        Assert.NotNull(deleted);
        Assert.Equal(created.ClassId, deleted.ClassId);
        Assert.Equal(created.UserId, deleted.UserId);

        var found = await _reader.GetAsync(id);
        Assert.Null(found);
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
