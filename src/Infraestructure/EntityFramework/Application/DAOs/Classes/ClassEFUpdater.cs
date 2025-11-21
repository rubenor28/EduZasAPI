using Application.DTOs.Classes;
using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers;
using InterfaceAdapters.Mappers.Common;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Application.DAOs.Classes;

public class ClassEFUpdater(
    EduZasDotnetContext ctx,
    IMapper<Class, ClassDomain> domainMapper,
    IUpdateMapper<ClassUpdateDTO, Class> updateMapper
) : EFUpdater<ClassDomain, ClassUpdateDTO, Class>(ctx, domainMapper, updateMapper)
{
    protected override async Task<Class?> GetTrackedByDTO(ClassUpdateDTO value) =>
        await _dbSet
            .AsTracking()
            .AsQueryable()
            .Where(c => c.ClassId == value.Id)
            .FirstOrDefaultAsync();
}
