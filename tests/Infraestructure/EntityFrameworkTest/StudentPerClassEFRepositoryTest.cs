using Application.DTOs.ClassStudents;
using EntityFramework.Application.DAOs.ClassStudents;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers;
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
    private readonly ClassStudentEFQuerier _querier;

    public StudentPerClassEFRepositoryTest()
    {
        var dbName = Guid.NewGuid().ToString();
        _conn = new SqliteConnection($"Data Source={dbName};Mode=Memory;Cache=Shared");
        _conn.Open();

        var opts = new DbContextOptionsBuilder<EduZasDotnetContext>().UseSqlite(_conn).Options;

        _ctx = new EduZasDotnetContext(opts);
        _ctx.Database.EnsureCreated();

        var mapper = new StudentClassEFMapper();

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
        var newRelation = new StudentClassRelationDTO
        {
            Id = new() { ClassId = "test-class", UserId = 1 },
            Hidden = false,
        };

        var created = await _creator.AddAsync(newRelation);

        Assert.NotNull(created);
        Assert.Equal(newRelation.Id.ClassId, created.Id.ClassId);
        Assert.Equal(newRelation.Id.UserId, created.Id.UserId);
        Assert.Equal(newRelation.Hidden, created.Hidden);
    }

    [Fact]
    public async Task Add_DuplicateRelation_ThrowsException()
    {
        await SeedData();
        var newRelation = new StudentClassRelationDTO
        {
            Id = new() { ClassId = "test-class", UserId = 1 },
            Hidden = false,
        };
        await _creator.AddAsync(newRelation);

        await Assert.ThrowsAnyAsync<Exception>(() => _creator.AddAsync(newRelation));
    }

    [Fact]
    public async Task Get_ExistingRelation_ReturnsRelation()
    {
        await SeedData();
        var newRelation = new StudentClassRelationDTO
        {
            Id = new() { ClassId = "test-class", UserId = 1 },
            Hidden = false,
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
        var newRelation = new StudentClassRelationDTO
        {
            Id = new() { ClassId = "test-class", UserId = 1 },
            Hidden = false,
        };
        await _creator.AddAsync(newRelation);

        var toUpdate = new StudentClassRelationDTO { Id = newRelation.Id, Hidden = true };

        var updated = await _updater.UpdateAsync(toUpdate);

        Assert.NotNull(updated);
        Assert.True(updated.Hidden);
    }

    [Fact]
    public async Task Update_NonExistingRelation_ThrowsException()
    {
        var toUpdate = new StudentClassRelationDTO
        {
            Id = new() { ClassId = "non-existent", UserId = 99 },
            Hidden = true,
        };

        await Assert.ThrowsAsync<ArgumentException>(() => _updater.UpdateAsync(toUpdate));
    }

    [Fact]
    public async Task Delete_ExistingRelation_ReturnsDeletedRelation()
    {
        await SeedData();
        var newRelation = new StudentClassRelationDTO
        {
            Id = new() { ClassId = "test-class", UserId = 1 },
            Hidden = false,
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
        var newRelation = new StudentClassRelationDTO
        {
            Id = new() { ClassId = "test-class", UserId = 1 },
            Hidden = false,
        };
        await _creator.AddAsync(newRelation);

        var criteria = new StudentClassRelationCriteriaDTO { UserId = 1 };

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
