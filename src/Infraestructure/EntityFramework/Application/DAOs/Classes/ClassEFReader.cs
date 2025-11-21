using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Application.DAOs.Classes;

public class ClassEFReader(EduZasDotnetContext ctx, IMapper<Class, ClassDomain> domainMapper)
    : EFReader<string, ClassDomain, Class>(ctx, domainMapper)
{
    public override Task<Class?> GetTrackedById(string id) =>
        _dbSet.AsTracking().AsQueryable().Where(c => c.ClassId == id).FirstOrDefaultAsync();
}
