using Domain.Enums;

namespace Application.DTOs.Common;

/// <summary>
/// Representa al usuario que ejecuta una acción en un caso de uso.
/// </summary>
/// <remarks>
/// Este DTO se utiliza para encapsular la información del actor que realiza
/// una operación, incluyendo su identificador y su rol en el sistema.
/// </remarks>
public sealed record class Executor
{
    /// <summary>
    /// Obtiene o establece el identificador único del usuario que ejecuta la acción.
    /// </summary>
    public required ulong Id { get; set; }

    /// <summary>
    /// Obtiene o establece el rol del usuario que ejecuta la acción.
    /// </summary>
    public required UserType Role { get; set; }
}
