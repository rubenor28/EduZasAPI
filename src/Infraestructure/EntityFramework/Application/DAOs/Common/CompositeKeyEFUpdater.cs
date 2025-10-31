using Application.DAOs;
using Domain.ValueObjects;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.Application.DAOs.Common;

public abstract class CompositeKeyEFUpdater<I, DomainEntity, UpdateEntity, EFEntity>(
    EduZasDotnetContext ctx,
    IMapper<EFEntity, DomainEntity> domainMapper,
    IUpdateMapper<UpdateEntity, EFEntity> updateMapper
)
    : EntityFrameworkDAO<EFEntity, DomainEntity>(ctx, domainMapper),
        IUpdaterAsync<DomainEntity, UpdateEntity>
    where EFEntity : class
    where I : notnull
    where DomainEntity : IIdentifiable<I>
    where UpdateEntity : IIdentifiable<I>
{
    protected readonly IUpdateMapper<UpdateEntity, EFEntity> _updateMapper = updateMapper;

    public async Task<DomainEntity> UpdateAsync(UpdateEntity updateData)
    {
        var tracked =
            await GetTrackedById(updateData.Id)
            ?? throw new ArgumentException($"Entity with the provided id not found");

        _updateMapper.Map(updateData, tracked);

        await _ctx.SaveChangesAsync();
        return _domainMapper.Map(tracked);
    }

    protected abstract Task<EFEntity?> GetTrackedById(I id);
}
