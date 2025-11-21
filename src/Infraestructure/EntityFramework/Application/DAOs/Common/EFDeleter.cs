using Application.DAOs;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.Application.DAOs.Common;

public abstract class EFDeleter<I, DomainEntity, EFEntity>(
    EduZasDotnetContext ctx,
    IMapper<EFEntity, DomainEntity> domainMapper
) : EntityFrameworkDAO<DomainEntity, EFEntity>(ctx, domainMapper), IDeleterAsync<I, DomainEntity>
    where I : notnull
    where DomainEntity : notnull
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
