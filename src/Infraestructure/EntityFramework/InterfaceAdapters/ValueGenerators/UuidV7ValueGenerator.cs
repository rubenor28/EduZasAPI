using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace EntityFramework.InterfaceAdapters.ValueGenerators;

/// <summary>
/// Generador de valores para crear GUIDs v7, que son ordenables y
/// óptimos para claves primarias en bases de datos.
/// </summary>
public class GuidV7ValueGenerator : ValueGenerator<Guid>
{
    /// <summary>
    /// Indica que este generador crea los valores, por lo que no se deben
    /// generar valores temporales.
    /// </summary>
    public override bool GeneratesTemporaryValues => false;

    /// <summary>
    /// Genera un nuevo GUID v7.
    /// </summary>
    /// <param name="entry">La entrada de seguimiento de cambios de EF Core.</param>
    /// <returns>Un nuevo Guid v7.</returns>
    public override Guid Next(EntityEntry entry) => Guid.CreateVersion7();
}
