using System.Linq.Expressions;
using Application.DAOs;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Application.DAOs.Common;

/// <summary>
/// Implementación base para leer entidades por ID usando EF.
/// </summary>
/// <typeparam name="I">Tipo del ID.</typeparam>
/// <typeparam name="DomainEntity">Entidad de dominio.</typeparam>
/// <typeparam name="EFEntity">Entidad de EF.</typeparam>
public abstract class EFReader<I, DomainEntity, EFEntity>(
    EduZasDotnetContext ctx,
    IMapper<EFEntity, DomainEntity> mapper
) : EntityFrameworkDAO<DomainEntity, EFEntity>(ctx), IReaderAsync<I, DomainEntity>
    where I : notnull
    where EFEntity : class
    where DomainEntity : notnull
{
    private readonly IMapper<EFEntity, DomainEntity> _mapper = mapper;

    /// <inheritdoc/>
    public Task<DomainEntity?> GetAsync(I id) =>
        _dbSet
            .AsNoTracking()
            .Where(GetIdPredicate(id))
            .Select(e => _mapper.Map(e))
            .FirstOrDefaultAsync();

    /// <summary>
    /// Obtiene el predicado para filtrar por ID.
    /// </summary>
    protected abstract Expression<Func<EFEntity, bool>> GetIdPredicate(I id);
}
