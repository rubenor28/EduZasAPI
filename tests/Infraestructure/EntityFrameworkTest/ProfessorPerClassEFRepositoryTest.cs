using Application.DTOs.ClassProfessors;
using EntityFramework.Application.DAOs.ClassProfessors;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkTest;

public class ProfessorPerClassEFRepositoryTest : IDisposable
{
    private readonly EduZasDotnetContext _ctx;
    private readonly SqliteConnection _conn;
    private readonly ClassProfessorEFCreator _creator;
    private readonly ClassProfessorsEFReader _reader;
    private readonly ClassProfessorsEFUpdater _updater;
    private readonly ClassProfessorsEFDeleter _deleter;
    private readonly ClassProfessorsEFQuerier _querier;

    public ProfessorPerClassEFRepositoryTest()
    {
        var dbName = Guid.NewGuid().ToString();
        _conn = new SqliteConnection($"Data Source={dbName};Mode=Memory;Cache=Shared");
        _conn.Open();

        var opts = new DbContextOptionsBuilder<EduZasDotnetContext>().UseSqlite(_conn).Options;

        _ctx = new EduZasDotnetContext(opts);
        _ctx.Database.EnsureCreated();

        var mapper = new ProfessorClassEFMapper();

        _creator = new(_ctx, mapper, mapper);
        _reader = new(_ctx, mapper);
        _updater = new(_ctx, mapper, mapper);
        _deleter = new(_ctx, mapper);
        _querier = new(_ctx, mapper, 10);
    }

    private async Task SeedData()
    {
        var user = new User
        {
            UserId = 1,
            Email = "test@test.com",
            FirstName = "test",
            FatherLastname = "test",
            Password = "test",
        };
        var classEntity = new Class { ClassId = "test-class", ClassName = "Test Class" };
        _ctx.Users.Add(user);
        _ctx.Classes.Add(classEntity);
        await _ctx.SaveChangesAsync();
    }

    [Fact]
    public async Task Add_ValidRelation_ReturnsRelation()
    {
        await SeedData();
        var newRelation = new ProfessorClassRelationDTO
        {
            Id = new() { ClassId = "test-class", UserId = 1 },
            IsOwner = true,
        };

        var created = await _creator.AddAsync(newRelation);

        Assert.NotNull(created);
        Assert.Equal(newRelation.Id.ClassId, created.Id.ClassId);
        Assert.Equal(newRelation.Id.UserId, created.Id.UserId);
        Assert.Equal(newRelation.IsOwner, created.IsOwner);
    }

    [Fact]
    public async Task Add_DuplicateRelation_ThrowsException()
    {
        await SeedData();
        var newRelation = new ProfessorClassRelationDTO
        {
            Id = new() { ClassId = "test-class", UserId = 1 },
            IsOwner = true,
        };
        await _creator.AddAsync(newRelation);

        await Assert.ThrowsAnyAsync<Exception>(() => _creator.AddAsync(newRelation));
    }

    [Fact]
    public async Task Get_ExistingRelation_ReturnsRelation()
    {
        await SeedData();
        var newRelation = new ProfessorClassRelationDTO
        {
            Id = new() { ClassId = "test-class", UserId = 1 },
            IsOwner = true,
        };
        await _creator.AddAsync(newRelation);

        var found = await _reader.GetAsync(newRelation.Id);

        Assert.True(found.IsSome);
        Assert.Equal(newRelation.Id, found.Unwrap().Id);
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
        await SeedData();
        var newRelation = new ProfessorClassRelationDTO
        {
            Id = new() { ClassId = "test-class", UserId = 1 },
            IsOwner = true,
        };
        await _creator.AddAsync(newRelation);

        var toUpdate = new ProfessorClassRelationDTO { Id = newRelation.Id, IsOwner = false };

        var updated = await _updater.UpdateAsync(toUpdate);

        Assert.NotNull(updated);
        Assert.False(updated.IsOwner);
    }

    [Fact]
    public async Task Update_NonExistingRelation_ThrowsException()
    {
        var toUpdate = new ProfessorClassRelationDTO
        {
            Id = new() { ClassId = "non-existent", UserId = 99 },
            IsOwner = false,
        };

        await Assert.ThrowsAsync<ArgumentException>(() => _updater.UpdateAsync(toUpdate));
    }

    [Fact]
    public async Task Delete_ExistingRelation_ReturnsDeletedRelation()
    {
        await SeedData();
        var newRelation = new ProfessorClassRelationDTO
        {
            Id = new() { ClassId = "test-class", UserId = 1 },
            IsOwner = true,
        };
        await _creator.AddAsync(newRelation);

        var deleted = await _deleter.DeleteAsync(newRelation.Id);

        Assert.NotNull(deleted);
        Assert.Equal(newRelation.Id, deleted.Id);

        var found = await _reader.GetAsync(newRelation.Id);
        Assert.True(found.IsNone);
    }

    [Fact]
    public async Task Delete_NonExistingRelation_ThrowsException()
    {
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _deleter.DeleteAsync(new() { ClassId = "non-existent", UserId = 99 })
        );
    }

    [Fact]
    public async Task GetBy_WithUserId_ReturnsMatchingRelations()
    {
        await SeedData();
        var newRelation = new ProfessorClassRelationDTO
        {
            Id = new() { ClassId = "test-class", UserId = 1 },
            IsOwner = true,
        };
        await _creator.AddAsync(newRelation);

        var criteria = new ProfessorClassRelationCriteriaDTO { UserId = 1 };

        var result = await _querier.GetByAsync(criteria);

        Assert.Single(result.Results);
        Assert.Equal(newRelation.Id, result.Results.First().Id);
    }

    public void Dispose()
    {
        _conn.Close();
        _conn.Dispose();
        _ctx.Dispose();
    }
}
