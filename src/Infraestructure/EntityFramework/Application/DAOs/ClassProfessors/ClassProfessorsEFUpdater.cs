using Application.DTOs.ClassProfessors;
using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Common;
using InterfaceAdapters.Mappers.Common;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Application.DAOs.ClassProfessors;

/// <summary>
/// Implementación de actualización de relaciones Clase-Profesor usando EF.
/// </summary>
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
    /// <summary>
    /// Obtiene la entidad de relación profesor-clase rastreada a partir del DTO.
    /// </summary>
    /// <param name="value">DTO de actualización.</param>
    /// <returns>Entidad rastreada o null.</returns>
    protected override Task<ClassProfessor?> GetTrackedByDTO(ClassProfessorUpdateDTO value) =>
        await _dbSet
            .AsTracking()
            .AsQueryable()
            .Where(cs => cs.ProfessorId == value.UserId)
            .Where(cs => cs.ClassId == value.ClassId)
            .FirstOrDefaultAsync();
}
