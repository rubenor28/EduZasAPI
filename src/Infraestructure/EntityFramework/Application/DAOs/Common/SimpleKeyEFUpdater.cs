using Application.DAOs;
using Domain.ValueObjects;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers;
using InterfaceAdapters.Mappers.Common;

public class SimpleKeyEFUpdater<I, DomainEntity, UpdateDTO, EFEntity>
    : EntityFrameworkDAO<EFEntity, DomainEntity>,
        IUpdaterAsync<DomainEntity, UpdateDTO>
    where I : notnull
    where UpdateDTO : IIdentifiable<I>
    where EFEntity : class
    where DomainEntity : notnull
{
    protected readonly IUpdateMapper<UpdateDTO, EFEntity> _updateMapper;

    public SimpleKeyEFUpdater(
        EduZasDotnetContext ctx,
        IMapper<EFEntity, DomainEntity> domainMapper,
        IUpdateMapper<UpdateDTO, EFEntity> updateMapper
    )
        : base(ctx, domainMapper)
    {
        _updateMapper = updateMapper;
    }

    public async Task<DomainEntity> UpdateAsync(UpdateDTO updateData)
    {
        var tracked = await _dbSet.FindAsync(updateData.Id);
        if (tracked is null)
            throw new ArgumentException($"Entity with the provided id not found");

        _updateMapper.Map(updateData, tracked);

        await _ctx.SaveChangesAsync();
        return _domainMapper.Map(tracked);
    }
}
