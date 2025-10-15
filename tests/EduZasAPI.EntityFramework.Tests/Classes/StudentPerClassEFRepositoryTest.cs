
using EduZasAPI.Domain.Common;
using EduZasAPI.Application.Common;
using EduZasAPI.Application.Classes;
using EduZasAPI.Infraestructure.EntityFramework.Application.Common;
using EduZasAPI.Infraestructure.EntityFramework.Application.Classes;
using EduZasAPI.Infraestructure.EntityFramework.Application.Users;

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace EduZasAPI.Tests.EntityFramework;

public class StudentPerClassEFRepositoryTest : IDisposable
{
    private readonly IRepositoryAsync<ClassUserRelationIdDTO, StudentClassRelationDTO, StudentClassRelationDTO, StudentClassRelationDTO, StudentClassRelationDTO, StudentClassRelationCriteriaDTO> _repository;
    private readonly EduZasDotnetContext _ctx;
    private readonly SqliteConnection _conn;

    public StudentPerClassEFRepositoryTest()
    {
        _conn = new SqliteConnection("Data Source=:memory:");
        _conn.Open();

        var opts = new DbContextOptionsBuilder<EduZasDotnetContext>()
          .UseSqlite(_conn)
          .Options;

        _ctx = new EduZasDotnetContext(opts);
        _ctx.Database.EnsureCreated();
        _repository = new StudentPerClassEntityFrameworkRepository(_ctx, 10);
    }

    private async Task SeedData()
    {
        var user = new User { UserId = 1, Email = "test@test.com", FirstName = "test", FatherLastname = "test", Password = "test" };
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
            Id = new ClassUserRelationIdDTO { ClassId = "test-class", UserId = 1 },
            Hidden = false
        };

        var created = await _repository.AddAsync(newRelation);

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
            Id = new ClassUserRelationIdDTO { ClassId = "test-class", UserId = 1 },
            Hidden = false
        };
        await _repository.AddAsync(newRelation);

        await Assert.ThrowsAsync<InvalidOperationException>(() => _repository.AddAsync(newRelation));
    }

    [Fact]
    public async Task Get_ExistingRelation_ReturnsRelation()
    {
        await SeedData();
        var newRelation = new StudentClassRelationDTO
        {
            Id = new ClassUserRelationIdDTO { ClassId = "test-class", UserId = 1 },
            Hidden = false
        };
        await _repository.AddAsync(newRelation);

        var found = await _repository.GetAsync(newRelation.Id);

        Assert.True(found.IsSome);
        Assert.Equal(newRelation.Id, found.Unwrap().Id);
    }

    [Fact]
    public async Task Get_NonExistingRelation_ReturnsEmptyOptional()
    {
        var found = await _repository.GetAsync(new ClassUserRelationIdDTO { ClassId = "non-existent", UserId = 99 });

        Assert.True(found.IsNone);
    }

    [Fact]
    public async Task Update_ExistingRelation_ReturnsUpdatedRelation()
    {
        await SeedData();
        var newRelation = new StudentClassRelationDTO
        {
            Id = new ClassUserRelationIdDTO { ClassId = "test-class", UserId = 1 },
            Hidden = false
        };
        await _repository.AddAsync(newRelation);

        var toUpdate = new StudentClassRelationDTO
        {
            Id = newRelation.Id,
            Hidden = true
        };

        var updated = await _repository.UpdateAsync(toUpdate);

        Assert.NotNull(updated);
        Assert.True(updated.Hidden);
    }

    [Fact]
    public async Task Update_NonExistingRelation_ThrowsException()
    {
        var toUpdate = new StudentClassRelationDTO
        {
            Id = new ClassUserRelationIdDTO { ClassId = "non-existent", UserId = 99 },
            Hidden = true
        };

        await Assert.ThrowsAsync<ArgumentException>(() => _repository.UpdateAsync(toUpdate));
    }

    [Fact]
    public async Task Delete_ExistingRelation_ReturnsDeletedRelation()
    {
        await SeedData();
        var newRelation = new StudentClassRelationDTO
        {
            Id = new ClassUserRelationIdDTO { ClassId = "test-class", UserId = 1 },
            Hidden = false
        };
        await _repository.AddAsync(newRelation);

        var deleted = await _repository.DeleteAsync(newRelation.Id);

        Assert.NotNull(deleted);
        Assert.Equal(newRelation.Id, deleted.Id);

        var found = await _repository.GetAsync(newRelation.Id);
        Assert.True(found.IsNone);
    }

    [Fact]
    public async Task Delete_NonExistingRelation_ThrowsException()
    {
        await Assert.ThrowsAsync<ArgumentException>(() => _repository.DeleteAsync(new ClassUserRelationIdDTO { ClassId = "non-existent", UserId = 99 }));
    }

    [Fact]
    public async Task GetBy_WithUserId_ReturnsMatchingRelations()
    {
        await SeedData();
        var newRelation = new StudentClassRelationDTO
        {
            Id = new ClassUserRelationIdDTO { ClassId = "test-class", UserId = 1 },
            Hidden = false
        };
        await _repository.AddAsync(newRelation);

        var criteria = new StudentClassRelationCriteriaDTO
        {
            UserId = Optional.Some<ulong>(1)
        };

        var result = await _repository.GetByAsync(criteria);

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
