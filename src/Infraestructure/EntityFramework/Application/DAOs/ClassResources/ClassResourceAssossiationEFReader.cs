using Application.DAOs;
using Application.DTOs.ClassResources;
using Domain.ValueObjects;
using EntityFramework.Application.DTOs;

namespace EntityFramework.Application.DAOs.ClassResources;

public sealed class ClassResourceAssociationReader(EduZasDotnetContext ctx)
    : IReaderAsync<Guid, IEnumerable<ClassResourceAssociationDTO>>
{
    private readonly EduZasDotnetContext _ctx = ctx;

    public Task<Optional<IEnumerable<ClassResourceAssociationDTO>>> GetAsync(Guid id)
    {
        throw new NotImplementedException();
    }
}
