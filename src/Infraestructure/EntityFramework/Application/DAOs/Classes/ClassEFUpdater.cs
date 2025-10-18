using Application.DTOs.Classes;
using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.Application.DAOs.Classes;

public class ClassEFUpdater(
    EduZasDotnetContext ctx,
    IMapper<Class, ClassDomain> domainMapper,
    IUpdateMapper<ClassUpdateDTO, Class> updateMapper
) : SimpleKeyEFUpdater<string, ClassDomain, ClassUpdateDTO, Class>(ctx, domainMapper, updateMapper);
