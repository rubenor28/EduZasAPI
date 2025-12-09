using Application.DAOs;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Common;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.Application.DAOs.Common;

/// <summary>
/// Implementación base para actualizar entidades usando EF.
/// </summary>
/// <typeparam name="DomainEntity">Entidad de dominio.</typeparam>
/// <typeparam name="UpdateEntity">DTO de actualización.</typeparam>
/// <typeparam name="EFEntity">Entidad de EF.</typeparam>
public abstract class EFUpdater<DomainEntity, UpdateEntity, EFEntity>(
    EduZasDotnetContext ctx,
    IMapper<EFEntity, DomainEntity> domainMapper,
    IUpdateMapper<UpdateEntity, EFEntity> updateMapper
) : EntityFrameworkDAO<DomainEntity, EFEntity>(ctx), IUpdaterAsync<DomainEntity, UpdateEntity>
    where EFEntity : class
    where DomainEntity : notnull
    where UpdateEntity : notnull
{
    protected readonly IMapper<EFEntity, DomainEntity> _domainMapper = domainMapper;
    protected readonly IUpdateMapper<UpdateEntity, EFEntity> _updateMapper = updateMapper;

    /// <inheritdoc/>
    public async Task<DomainEntity> UpdateAsync(UpdateEntity updateData)
    {
        var tracked =
            await GetTrackedByDTO(updateData)
            ?? throw new ArgumentException($"Entity with the provided id not found");

        _updateMapper.Map(updateData, tracked);

        await _ctx.SaveChangesAsync();
        return _domainMapper.Map(tracked);
    }

    /// <summary>
    /// Obtiene la entidad rastreada a partir del DTO de actualización.
    /// </summary>
    protected abstract Task<EFEntity?> GetTrackedByDTO(UpdateEntity value);
}
