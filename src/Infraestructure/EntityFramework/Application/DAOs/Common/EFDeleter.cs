using Application.DAOs;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.Application.DAOs.Common;

public abstract class EFDeleter<I, DomainEntity, EFEntity>(
    EduZasDotnetContext ctx,
    IMapper<EFEntity, DomainEntity> domainMapper
) : EntityFrameworkDAO<DomainEntity, EFEntity>(ctx), IDeleterAsync<I, DomainEntity>
    where I : notnull
    where DomainEntity : notnull
    where EFEntity : class
{
    private readonly IMapper<EFEntity, DomainEntity> _domainMapper = domainMapper;

    public async Task<IEnumerable<DomainEntity>> BulkDelete(IEnumerable<I> ids)
    {
        var findTasks = ids.Select(async id =>
            await GetTrackedById(id) ?? throw new ArgumentException("Error al obtener registros")
        );

        var records =
            await Task.WhenAll(findTasks)
            ?? throw new ArgumentException("Error al eliminar registros");
        _dbSet.RemoveRange(records);

        return records.Select(_domainMapper.Map);
    }

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
