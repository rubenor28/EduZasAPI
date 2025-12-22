using Application.DAOs;
using Application.DTOs.ContactTags;
using Application.DTOs.Common;
using Domain.Enums;
using Microsoft.Extensions.DependencyInjection;
using Domain.Entities;

namespace Tests.Application.DAOs;

public class ContactTagEFRepositoryTest : BaseTest
{
    private readonly ICreatorAsync<ContactTagDomain, NewContactTagDTO> _creator;
    private readonly IReaderAsync<ContactTagIdDTO, ContactTagDomain> _reader;
    private readonly IDeleterAsync<ContactTagIdDTO, ContactTagDomain> _deleter;

    public ContactTagEFRepositoryTest()
    {
        _creator = _sp.GetRequiredService<ICreatorAsync<ContactTagDomain, NewContactTagDTO>>();
        _reader = _sp.GetRequiredService<IReaderAsync<ContactTagIdDTO, ContactTagDomain>>();
        _deleter = _sp.GetRequiredService<IDeleterAsync<ContactTagIdDTO, ContactTagDomain>>();
    }

    [Fact]
    public async Task Add_ValidRelation_ReturnsRelation()
    {
        // Arrange
        var owner = await SeedUser(UserType.PROFESSOR);
        var contact = await SeedUser(UserType.PROFESSOR);
        await SeedContact(owner.Id, contact.Id);
        var tag = await SeedTag("test-tag");
        
        var newRelationDto = new NewContactTagDTO
        {
            AgendaOwnerId = owner.Id,
            UserId = contact.Id,
            TagId = tag.Id,
        };

        // Act
        var created = await _creator.AddAsync(newRelationDto);

        // Assert
        Assert.NotNull(created);
        Assert.Equal(newRelationDto.AgendaOwnerId, created.Id.AgendaOwnerId);
        Assert.Equal(newRelationDto.UserId, created.Id.UserId);
        Assert.Equal(newRelationDto.TagId, created.Id.TagId);
    }

    [Fact]
    public async Task Add_DuplicateRelation_ThrowsException()
    {
        // Arrange
        var owner = await SeedUser(UserType.PROFESSOR);
        var contact = await SeedUser(UserType.PROFESSOR);
        await SeedContact(owner.Id, contact.Id);
        var tag = await SeedTag("test-tag");
        
        var newRelationDto = new NewContactTagDTO
        {
            AgendaOwnerId = owner.Id,
            UserId = contact.Id,
            TagId = tag.Id,
        };

        await _creator.AddAsync(newRelationDto);

        // Act & Assert
        await Assert.ThrowsAnyAsync<Exception>(() => _creator.AddAsync(newRelationDto));
    }

    [Fact]
    public async Task Get_ExistingRelation_ReturnsRelation()
    {
        // Arrange
        var owner = await SeedUser(UserType.PROFESSOR);
        var contact = await SeedUser(UserType.PROFESSOR);
        await SeedContact(owner.Id, contact.Id);
        var tag = await SeedTag("test-tag");
        await SeedContactTag(owner.Id, contact.Id, tag.Id);
        
        var idToFind = new ContactTagIdDTO
        {
            AgendaOwnerId = owner.Id,
            UserId = contact.Id,
            TagId = tag.Id,
        };

        // Act
        var found = await _reader.GetAsync(idToFind);

        // Assert
        Assert.NotNull(found);
        Assert.Equal(idToFind.AgendaOwnerId, found.Id.AgendaOwnerId);
        Assert.Equal(idToFind.UserId, found.Id.UserId);
        Assert.Equal(idToFind.TagId, found.Id.TagId);
    }

    [Fact]
    public async Task Get_NonExistingRelation_ReturnsEmptyOptional()
    {
        // Act
        var found = await _reader.GetAsync(
            new()
            {
                AgendaOwnerId = 99,
                UserId = 98,
                TagId = 97,
            }
        );

        // Assert
        Assert.Null(found);
    }

    [Fact]
    public async Task Delete_ExistingRelation_ReturnsDeletedRelation()
    {
        // Arrange
        var owner = await SeedUser(UserType.PROFESSOR);
        var contact = await SeedUser(UserType.PROFESSOR);
        await SeedContact(owner.Id, contact.Id);
        var tag = await SeedTag("test-tag");
        await SeedContactTag(owner.Id, contact.Id, tag.Id);
        
        var idToDelete = new ContactTagIdDTO
        {
            AgendaOwnerId = owner.Id,
            UserId = contact.Id,
            TagId = tag.Id,
        };

        // Act
        var deleted = await _deleter.DeleteAsync(idToDelete);
        var found = await _reader.GetAsync(idToDelete);

        // Assert
        Assert.NotNull(deleted);
        Assert.Equal(idToDelete.AgendaOwnerId, deleted.Id.AgendaOwnerId);
        Assert.Equal(idToDelete.UserId, deleted.Id.UserId);
        Assert.Equal(idToDelete.TagId, deleted.Id.TagId);
        Assert.Null(found);
    }

    [Fact]
    public async Task Delete_NonExistingRelation_ThrowsException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _deleter.DeleteAsync(
                new()
                {
                    AgendaOwnerId = 99,
                    UserId = 98,
                    TagId = 97,
                }
            )
        );
    }
}

