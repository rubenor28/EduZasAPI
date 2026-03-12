using Domain.Entities;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.Classes;

/// <summary>
/// Mapeador de entidad EF a dominio para clases.
/// </summary>
public class ClassMapper : IMapper<Class, ClassDomain>
{
    /// <summary>
    /// Mapea una entidad de clase de base de datos a un objeto de dominio.
    /// </summary>
    /// <param name="ef">Entidad de base de datos.</param>
    /// <returns>Objeto de dominio de clase.</returns>
    public ClassDomain Map(Class ef) =>
        new()
        {
            Id = ef.ClassId,
            Active = ef.Active ?? false,
            ClassName = ef.ClassName,
            Color = ef.Color ?? "#007bff",
            Subject = ef.Subject,
            Section = ef.Section,
            CreatedAt = ef.CreatedAt,
            ModifiedAt = ef.ModifiedAt,
        };
}
