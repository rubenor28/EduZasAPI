using Application.DAOs;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.Application.DAOs.Common;

public class EFBulkCreator<DomainEntity, NewEntity, EFEntity>(
    EduZasDotnetContext ctx,
    IMapper<EFEntity, DomainEntity> domainMapper,
    IMapper<NewEntity, EFEntity> newEntityMapper
)
    : EntityFrameworkDAO<EFEntity, DomainEntity>(ctx, domainMapper),
        IBulkCreatorAsync<DomainEntity, NewEntity>
    where EFEntity : class
    where NewEntity : notnull
    where DomainEntity : notnull
{
    protected readonly IMapper<NewEntity, EFEntity> _newEntityMapper = newEntityMapper;

    public async Task<IEnumerable<DomainEntity>> AddAsync(IEnumerable<NewEntity> data)
    {
        var entities = data.Select(_newEntityMapper.Map);
        await _dbSet.AddRangeAsync(entities);
        await _ctx.SaveChangesAsync();
        return entities.Select(_domainMapper.Map);
    }
}
