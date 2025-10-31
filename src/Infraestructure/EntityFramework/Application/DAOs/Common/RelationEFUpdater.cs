using Domain.ValueObjects;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.Application.DAOs.Common;

public abstract class RelationEFUpdater<I, DomainEntity, EFEntity>(
    EduZasDotnetContext ctx,
    IMapper<EFEntity, DomainEntity> domainMapper,
    IUpdateMapper<DomainEntity, EFEntity> updateMapper
) : CompositeKeyEFUpdater<I, DomainEntity, DomainEntity, EFEntity>(ctx, domainMapper, updateMapper)
    where EFEntity : class
    where I : notnull
    where DomainEntity : IIdentifiable<I>;
