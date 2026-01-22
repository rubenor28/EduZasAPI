using Application.DAOs;
using Application.DTOs.ClassTests;
using Domain.Entities;
using Domain.Enums;
using Microsoft.Extensions.DependencyInjection;

namespace Tests.Application.DAOs;

public class ClassTestRepositoryTest : BaseTest
{
    private readonly ICreatorAsync<ClassTestDomain, ClassTestIdDTO> _creator;
    private readonly IDeleterAsync<ClassTestIdDTO, ClassTestDomain> _deleter;
    private readonly IReaderAsync<ClassTestIdDTO, ClassTestDomain> _reader;

    public ClassTestRepositoryTest()
    {
        _creator = _sp.GetRequiredService<ICreatorAsync<ClassTestDomain, ClassTestIdDTO>>();
        _deleter = _sp.GetRequiredService<IDeleterAsync<ClassTestIdDTO, ClassTestDomain>>();
        _reader = _sp.GetRequiredService<IReaderAsync<ClassTestIdDTO, ClassTestDomain>>();
    }

    [Fact]
    public async Task AddClassTest_ReturnsClassTest()
    {
        // Arrange
        var professor = await SeedUser(UserType.PROFESSOR);
        var createdClass = await SeedClass(professor.Id);
        var createdTest = await SeedTest(professor.Id);

        var newClassTest = new ClassTestIdDTO
        {
            ClassId = createdClass.Id,
            TestId = createdTest.Id,
        };

        // Act
        var created = await _creator.AddAsync(newClassTest);

        // Assert
        Assert.NotNull(created);
        Assert.Equal(newClassTest.ClassId, created.ClassId);
        Assert.Equal(newClassTest.TestId, created.TestId);
    }

    [Fact]
    public async Task DeleteClassTest_ShouldRemoveRelation()
    {
        // Arrange
        var professor = await SeedUser(UserType.PROFESSOR);
        var createdClass = await SeedClass(professor.Id);
        var createdTest = await SeedTest(professor.Id);

        var newClassTestDto = new ClassTestIdDTO
        {
            ClassId = createdClass.Id,
            TestId = createdTest.Id,
        };

        await _creator.AddAsync(newClassTestDto);

        var idDto = new ClassTestIdDTO { ClassId = createdClass.Id, TestId = createdTest.Id };

        // Act
        var deleted = await _deleter.DeleteAsync(idDto);
        var found = await _reader.GetAsync(idDto);

        // Assert
        Assert.NotNull(deleted);
        Assert.Null(found);
    }
}

