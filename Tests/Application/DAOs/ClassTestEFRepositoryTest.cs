using Application.DAOs;
using Application.DTOs.ClassTests;
using Application.DTOs.Common;
using Domain.Entities;
using Domain.Enums;
using Microsoft.Extensions.DependencyInjection;

namespace Tests.Application.DAOs;

public class ClassTestEFRepositoryTest : BaseTest
{
    private readonly ICreatorAsync<ClassTestDomain, ClassTestDTO> _creator;
    private readonly IUpdaterAsync<ClassTestDomain, ClassTestDTO> _updater;
    private readonly IDeleterAsync<ClassTestIdDTO, ClassTestDomain> _deleter;
    private readonly IReaderAsync<ClassTestIdDTO, ClassTestDomain> _reader;

    public ClassTestEFRepositoryTest()
    {
        _creator = _sp.GetRequiredService<ICreatorAsync<ClassTestDomain, ClassTestDTO>>();
        _updater = _sp.GetRequiredService<IUpdaterAsync<ClassTestDomain, ClassTestDTO>>();
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

        var newClassTest = new ClassTestDTO
        {
            ClassId = createdClass.Id,
            TestId = createdTest.Id,
            Visible = true,
        };

        // Act
        var created = await _creator.AddAsync(newClassTest);

        // Assert
        Assert.NotNull(created);
        Assert.Equal(newClassTest.ClassId, created.ClassId);
        Assert.Equal(newClassTest.TestId, created.TestId);
    }

    [Fact]
    public async Task UpdateClassTest_ReturnsUpdatedClassTest()
    {
        // Arrange
        var professor = await SeedUser(UserType.PROFESSOR);
        var createdClass = await SeedClass(professor.Id);
        var createdTest = await SeedTest(professor.Id);

        var newClassTestDto = new ClassTestDTO
        {
            ClassId = createdClass.Id,
            TestId = createdTest.Id,
            Visible = true,
        };
        await _creator.AddAsync(newClassTestDto);

        var updateDto = new ClassTestDTO
        {
            ClassId = createdClass.Id,
            TestId = createdTest.Id,
            Visible = false,
        };

        // Act
        var updatedClassTest = await _updater.UpdateAsync(updateDto);

        // Assert
        Assert.NotNull(updatedClassTest);
        Assert.Equal(updateDto.Visible, updatedClassTest.Visible);
    }

    [Fact]
    public async Task DeleteClassTest_ShouldRemoveRelation()
    {
        // Arrange
        var professor = await SeedUser(UserType.PROFESSOR);
        var createdClass = await SeedClass(professor.Id);
        var createdTest = await SeedTest(professor.Id);

        var newClassTestDto = new ClassTestDTO
        {
            ClassId = createdClass.Id,
            TestId = createdTest.Id,
            Visible = true,
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

