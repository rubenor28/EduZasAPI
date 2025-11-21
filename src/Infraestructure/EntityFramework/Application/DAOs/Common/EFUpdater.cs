using Application.DAOs;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.Application.DAOs.Common;

public abstract class EFUpdater<DomainEntity, UpdateEntity, EFEntity>(
    EduZasDotnetContext ctx,
    IMapper<EFEntity, DomainEntity> domainMapper,
    IUpdateMapper<UpdateEntity, EFEntity> updateMapper
)
    : EntityFrameworkDAO<DomainEntity, EFEntity>(ctx, domainMapper),
        IUpdaterAsync<DomainEntity, UpdateEntity>
    where EFEntity : class
    where DomainEntity : notnull
    where UpdateEntity : notnull
{
    protected readonly IUpdateMapper<UpdateEntity, EFEntity> _updateMapper = updateMapper;

    public async Task<DomainEntity> UpdateAsync(UpdateEntity updateData)
    {
        var tracked =
            await GetTrackedByDTO(updateData)
            ?? throw new ArgumentException($"Entity with the provided id not found");

        _updateMapper.Map(updateData, tracked);

        await _ctx.SaveChangesAsync();
        return _domainMapper.Map(tracked);
    }

    protected abstract Task<EFEntity?> GetTrackedByDTO(UpdateEntity value);
}
