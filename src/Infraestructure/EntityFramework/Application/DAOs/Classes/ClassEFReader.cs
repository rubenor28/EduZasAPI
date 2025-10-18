using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.Application.DAOs.Classes;

public class ClassEFReader(EduZasDotnetContext ctx, IMapper<Class, ClassDomain> domainMapper)
    : SimpleKeyEFReader<string, ClassDomain, Class>(ctx, domainMapper);
