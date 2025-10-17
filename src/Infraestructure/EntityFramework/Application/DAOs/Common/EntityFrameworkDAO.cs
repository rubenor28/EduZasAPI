using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Application.DAOs.Common;

public abstract class EntityFrameworkDAO<EFEntity, DomainEntity>
    where EFEntity : class
{
    protected readonly EduZasDotnetContext _ctx;
    protected readonly IMapper<EFEntity, DomainEntity> _domainMapper;
    protected readonly DbSet<EFEntity> _dbSet;

    public EntityFrameworkDAO(EduZasDotnetContext ctx, IMapper<EFEntity, DomainEntity> domainMapper)
    {
        _ctx = ctx;
        _dbSet = ctx.Set<EFEntity>();
        _domainMapper = domainMapper;
    }
}
