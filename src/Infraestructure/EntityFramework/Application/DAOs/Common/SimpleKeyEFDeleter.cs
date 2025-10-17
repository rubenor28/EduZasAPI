using Application.DAOs;
using Domain.ValueObjects;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

public class SimpleKeyEFDeleter<I, DomainEntity, EFEntity>
    : EntityFrameworkDAO<EFEntity, DomainEntity>,
        IDeleterAsync<I, DomainEntity>
    where I : notnull
    where DomainEntity : IIdentifiable<I>
    where EFEntity : class
{
    public SimpleKeyEFDeleter(EduZasDotnetContext ctx, IMapper<EFEntity, DomainEntity> domainMapper)
        : base(ctx, domainMapper) { }

    public async Task<DomainEntity> DeleteAsync(I id)
    {
        var record = await _dbSet.FindAsync(id);
        if (record is null)
            throw new ArgumentException("Record do not exists");

        _dbSet.Remove(record);
        await _ctx.SaveChangesAsync();
        return _domainMapper.Map(record);
    }
}
