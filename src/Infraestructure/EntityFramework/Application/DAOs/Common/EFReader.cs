using Application.DAOs;
using Domain.ValueObjects;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.Application.DAOs.Common;

public abstract class EFReader<I, DomainEntity, EFEntity>(
    EduZasDotnetContext ctx,
    IMapper<EFEntity, DomainEntity> domainMapper
) : EntityFrameworkDAO<DomainEntity, EFEntity>(ctx, domainMapper), IReaderAsync<I, DomainEntity>
    where I : notnull
    where EFEntity : class
    where DomainEntity : notnull
{
    public async Task<Optional<DomainEntity>> GetAsync(I id)
    {
        var record = await GetTrackedById(id);
        return record is not null ? _domainMapper.Map(record) : Optional<DomainEntity>.None();
    }

    public abstract Task<EFEntity?> GetTrackedById(I id);
}
