using System.Linq.Expressions;
using Application.DAOs;
using Domain.ValueObjects;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Common;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Application.DAOs.Common;

public abstract class EFReader<I, DomainEntity, EFEntity>(
    EduZasDotnetContext ctx,
    IEFProjector<EFEntity, DomainEntity> projector
) : EntityFrameworkDAO<DomainEntity, EFEntity>(ctx, projector), IReaderAsync<I, DomainEntity>
    where I : notnull
    where EFEntity : class
    where DomainEntity : notnull
{
    private readonly IEFProjector<EFEntity, DomainEntity> _projector = projector;

    public async Task<Optional<DomainEntity>> GetAsync(I id)
    {
        var record = await _dbSet
            .AsNoTracking()
            .Where(GetIdPredicate(id))
            .Select(_projector.Projection)
            .FirstOrDefaultAsync();

        return record is not null ? record : Optional<DomainEntity>.None();
    }

    protected abstract Expression<Func<EFEntity, bool>> GetIdPredicate(I id);
}
