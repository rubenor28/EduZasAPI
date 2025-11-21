using Application.DTOs.ClassProfessors;
using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers;
using InterfaceAdapters.Mappers.Common;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Application.DAOs.ClassProfessors;

public class ClassProfessorsEFUpdater(
    EduZasDotnetContext ctx,
    IMapper<ClassProfessor, ClassProfessorDomain> domainMapper,
    IUpdateMapper<ClassProfessorUpdateDTO, ClassProfessor> updateMapper
)
    : EFUpdater<ClassProfessorDomain, ClassProfessorUpdateDTO, ClassProfessor>(
        ctx,
        domainMapper,
        updateMapper
    )
{
    protected override async Task<ClassProfessor?> GetTrackedByDTO(ClassProfessorUpdateDTO value) =>
        await _dbSet
            .AsTracking()
            .AsQueryable()
            .Where(cs => cs.ProfessorId == value.UserId)
            .Where(cs => cs.ClassId == value.ClassId)
            .FirstOrDefaultAsync();
}
