using EntityFramework.Application.DTOs;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Application.DAOs.Common;

/// <summary>
/// Clase base para DAOs de Entity Framework.
/// </summary>
/// <typeparam name="DomainEntity">Entidad de dominio.</typeparam>
/// <typeparam name="EFEntity">Entidad de EF.</typeparam>
public abstract class EntityFrameworkDAO<DomainEntity, EFEntity>(
    EduZasDotnetContext ctx
)
    where EFEntity : class
{
    protected readonly EduZasDotnetContext _ctx = ctx;
    protected readonly DbSet<EFEntity> _dbSet = ctx.Set<EFEntity>();
}
