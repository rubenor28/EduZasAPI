using Domain.Entities;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.Application.DAOs.Classes;

public class ClassEFDeleter : SimpleKeyEFDeleter<string, ClassDomain, Class>
{
    public ClassEFDeleter(EduZasDotnetContext ctx, IMapper<Class, ClassDomain> domainMapper)
        : base(ctx, domainMapper) { }
}
