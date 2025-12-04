using EntityFramework.Application.DTOs;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Application.DAOs.Common;

public abstract class EntityFrameworkDAO<DomainEntity, EFEntity>(
    EduZasDotnetContext ctx
)
    where EFEntity : class
{
    protected readonly EduZasDotnetContext _ctx = ctx;
    protected readonly DbSet<EFEntity> _dbSet = ctx.Set<EFEntity>();
}
