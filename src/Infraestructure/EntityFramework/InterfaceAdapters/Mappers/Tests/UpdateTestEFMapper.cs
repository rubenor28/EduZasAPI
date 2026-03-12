using Application.DTOs.Tests;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.Tests;

/// <summary>
/// Mapeador de actualización para exámenes.
/// </summary>
public class UpdateTestEFMapper : IUpdateMapper<TestUpdateDTO, Test>
{
    /// <summary>
    /// Actualiza una entidad de examen con los datos del DTO.
    /// </summary>
    /// <param name="tu">DTO de actualización.</param>
    /// <param name="t">Entidad de base de datos.</param>
    public void Map(TestUpdateDTO tu, Test t)
    {
        t.Title = tu.Title;
        t.Content = tu.Content;
        t.Color = tu.Color;
        t.TimeLimitMinutes = tu.TimeLimitMinutes;
        t.ProfessorId = tu.ProfessorId;
        t.Active = tu.Active;
    }
}
