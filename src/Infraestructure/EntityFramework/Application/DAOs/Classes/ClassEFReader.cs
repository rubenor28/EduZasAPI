using System.Linq.Expressions;
using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.Application.DAOs.Classes;

public class ClassEFReader(EduZasDotnetContext ctx, IMapper<Class, ClassDomain> mapper)
    : EFReader<string, ClassDomain, Class>(ctx, mapper)
{
    protected override Expression<Func<Class, bool>> GetIdPredicate(string id) =>
        c => c.ClassId == id;
}
