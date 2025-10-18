using Application.DAOs;
using Domain.ValueObjects;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.Application.DAOs.Common;

public abstract class CompositeKeyEFUpdater<I, DomainEntity, EFEntity>(
    EduZasDotnetContext ctx,
    IMapper<EFEntity, DomainEntity> domainMapper,
    IUpdateMapper<DomainEntity, EFEntity> updateMapper
)
    : EntityFrameworkDAO<EFEntity, DomainEntity>(ctx, domainMapper),
        IUpdaterAsync<DomainEntity, DomainEntity>
    where EFEntity : class
    where I : notnull
    where DomainEntity : IIdentifiable<I>
{
    protected readonly IUpdateMapper<DomainEntity, EFEntity> _updateMapper = updateMapper;

    public async Task<DomainEntity> UpdateAsync(DomainEntity updateData)
    {
        var tracked =
            await GetTrackedById(updateData.Id)
            ?? throw new ArgumentException($"Entity with the provided id not found");

        _updateMapper.Map(updateData, tracked);

        await _ctx.SaveChangesAsync();
        return _domainMapper.Map(tracked);
    }

    public abstract Task<EFEntity?> GetTrackedById(I id);
}
