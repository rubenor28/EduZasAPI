using Application.DAOs;
using Domain.ValueObjects;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.Application.DAOs.Common;

public abstract class CompositeKeyEFReader<I, DomainEntity, EFEntity>
    : EntityFrameworkDAO<EFEntity, DomainEntity>,
        IReaderAsync<I, DomainEntity>
    where I : notnull
    where EFEntity : class
    where DomainEntity : IIdentifiable<I>
{
    public CompositeKeyEFReader(EduZasDotnetContext ctx, IMapper<EFEntity, DomainEntity> domainMapper)
        : base(ctx, domainMapper) { }

    public async Task<Optional<DomainEntity>> GetAsync(I id)
    {
        var record = await GetTrackedById(id);
        if (record is null)
            return Optional<DomainEntity>.None();

        return _domainMapper.Map(record);
    }

    public abstract Task<EFEntity?> GetTrackedById(I id);
}
