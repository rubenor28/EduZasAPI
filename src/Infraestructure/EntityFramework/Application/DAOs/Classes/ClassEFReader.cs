using System.Linq.Expressions;
using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Common;

namespace EntityFramework.Application.DAOs.Classes;

public class ClassEFReader(EduZasDotnetContext ctx, IEFProjector<Class, ClassDomain> projector)
    : EFReader<string, ClassDomain, Class>(ctx, projector)
{
    protected override Expression<Func<Class, bool>> GetIdPredicate(string id) =>
        c => c.ClassId == id;
}
