using Application.DAOs;
using Domain.ValueObjects;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.Application.DAOs.Common;

public abstract class CompositeKeyEFDeleter<I, DomainEntity, EFEntity>(
    EduZasDotnetContext ctx,
    IMapper<EFEntity, DomainEntity> domainMapper
) : EntityFrameworkDAO<EFEntity, DomainEntity>(ctx, domainMapper), IDeleterAsync<I, DomainEntity>
    where I : notnull
    where DomainEntity : IIdentifiable<I>
    where EFEntity : class
{
    public async Task<DomainEntity> DeleteAsync(I id)
    {
        var record =
            await GetTrackedById(id) ?? throw new ArgumentException("Record do not exists");

        _dbSet.Remove(record);
        await _ctx.SaveChangesAsync();
        return _domainMapper.Map(record);
    }

    public abstract Task<EFEntity?> GetTrackedById(I id);
}
