using Application.DAOs;
using Domain.ValueObjects;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.Application.DAOs.Common;

public class SimpleKeyEFReader<I, DomainEntity, EFEntity>(
    EduZasDotnetContext ctx,
    IMapper<EFEntity, DomainEntity> domainMapper
) : EntityFrameworkDAO<EFEntity, DomainEntity>(ctx, domainMapper), IReaderAsync<I, DomainEntity>
    where I : notnull
    where EFEntity : class
    where DomainEntity : IIdentifiable<I>
{
    public async Task<Optional<DomainEntity>> GetAsync(I id)
    {
        var record = await _dbSet.FindAsync(id);
        if (record is null)
            return Optional<DomainEntity>.None();

        return _domainMapper.Map(record);
    }
}
