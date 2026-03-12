using Application.DAOs;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.Application.DAOs.Common;

/// <summary>
/// Implementación base para crear entidades usando EF.
/// </summary>
/// <typeparam name="DomainEntity">Entidad de dominio.</typeparam>
/// <typeparam name="NewEntity">DTO de creación.</typeparam>
/// <typeparam name="EFEntity">Entidad de EF.</typeparam>
public abstract class EFCreator<DomainEntity, NewEntity, EFEntity>(
    EduZasDotnetContext ctx,
    IMapper<EFEntity, DomainEntity> domainMapper,
    IMapper<NewEntity, EFEntity> newEntityMapper
)
    : EntityFrameworkDAO<DomainEntity, EFEntity>(ctx),
        ICreatorAsync<DomainEntity, NewEntity>,
        IBulkCreatorAsync<DomainEntity, NewEntity>
    where EFEntity : class
    where NewEntity : notnull
    where DomainEntity : notnull
{
    protected readonly IMapper<NewEntity, EFEntity> _newEntityMapper = newEntityMapper;
    protected readonly IMapper<EFEntity, DomainEntity> _domainMapper = domainMapper;

    /// <summary>
    /// Agrega una nueva entidad de forma asíncrona.
    /// </summary>
    public async Task<DomainEntity> AddAsync(NewEntity value)
    {
        var entity = _newEntityMapper.Map(value);
        await _dbSet.AddAsync(entity);
        await _ctx.SaveChangesAsync();
        return _domainMapper.Map(entity);
    }

    /// <summary>
    /// Agrega un rango de entidades de forma asíncrona.
    /// </summary>
    public async Task<IEnumerable<DomainEntity>> AddRangeAsync(IEnumerable<NewEntity> data)
    {
        var entities = data.Select(_newEntityMapper.Map);
        await _dbSet.AddRangeAsync(entities);
        await _ctx.SaveChangesAsync();
        return entities.Select(_domainMapper.Map);
    }
}
