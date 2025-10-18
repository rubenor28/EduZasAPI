using Application.DAOs;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.Application.DAOs.Common;

public class EFCreator<DomainEntity, NewEntity, EFEntity>(
    EduZasDotnetContext ctx,
    IMapper<EFEntity, DomainEntity> domainMapper,
    IMapper<NewEntity, EFEntity> newEntityMapper
)
    : EntityFrameworkDAO<EFEntity, DomainEntity>(ctx, domainMapper),
        ICreatorAsync<DomainEntity, NewEntity>
    where EFEntity : class
    where NewEntity : notnull
    where DomainEntity : notnull
{
    protected readonly IMapper<NewEntity, EFEntity> _newEntityMapper = newEntityMapper;

    public async Task<DomainEntity> AddAsync(NewEntity value)
    {
        var entity = _newEntityMapper.Map(value);
        await _dbSet.AddAsync(entity);
        await _ctx.SaveChangesAsync();
        return _domainMapper.Map(entity);
    }
}
