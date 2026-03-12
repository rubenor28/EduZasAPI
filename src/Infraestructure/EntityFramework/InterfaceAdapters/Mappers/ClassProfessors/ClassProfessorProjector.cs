using System.Linq.Expressions;
using Application.DTOs.ClassProfessors;
using Domain.Entities;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.ClassProfessors;

/// <summary>
/// Proyector de consultas para profesores de clase.
/// </summary>
public class ClassProfessorProjector
    : IEFProjector<ClassProfessor, ClassProfessorDomain, ClassProfessorCriteriaDTO>
{
    /// <summary>
    /// Obtiene la expresión de proyección para convertir una entidad de relación profesor-clase a un objeto de dominio.
    /// </summary>
    /// <param name="_">Criterios de consulta (ignorados).</param>
    /// <returns>Expresión de proyección.</returns>
    public Expression<Func<ClassProfessor, ClassProfessorDomain>> GetProjection(
        ClassProfessorCriteriaDTO _
    ) =>
        efEntity =>
            new()
            {
                ClassId = efEntity.ClassId,
                UserId = efEntity.ProfessorId,
                IsOwner = efEntity.IsOwner ?? false,
                CreatedAt = efEntity.CreatedAt,
            };
}
