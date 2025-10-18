using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Application.DAOs.Common;

public abstract class EntityFrameworkDAO<EFEntity, DomainEntity>(
    EduZasDotnetContext ctx,
    IMapper<EFEntity, DomainEntity> domainMapper
)
    where EFEntity : class
{
    protected readonly EduZasDotnetContext _ctx = ctx;
    protected readonly IMapper<EFEntity, DomainEntity> _domainMapper = domainMapper;
    protected readonly DbSet<EFEntity> _dbSet = ctx.Set<EFEntity>();
}
