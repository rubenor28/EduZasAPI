using Domain.Entities;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.Classes;

/// <summary>
/// Mapeador de entidad EF a dominio para clases.
/// </summary>
public class ClassMapper : IMapper<Class, ClassDomain>
{
    /// <inheritdoc/>
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
