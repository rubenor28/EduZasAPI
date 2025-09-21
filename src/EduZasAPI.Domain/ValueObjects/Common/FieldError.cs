namespace EduZasAPI.Domain.ValueObjects.Common;

/// <summary>
/// Representa un error de validación asociado a un campo específico en una operación.
/// </summary>
/// <remarks>
/// Esta estructura inmutable encapsula información detallada sobre errores de validación
/// que ocurren en campos particulares durante operaciones de entrada de datos, formularios
/// o procesamiento de solicitudes. Es ideal para devolver errores específicos por campo
/// en respuestas de API validaciones.
/// </remarks>
public struct FieldError
{
    /// <summary>
    /// Obtiene el nombre del campo que generó el error de validación.
    /// </summary>
    /// <value>Nombre del campo con error. Campo obligatorio.</value>
    public required string Field { get; init; }

    /// <summary>
    /// Obtiene el mensaje descriptivo que explica el error de validación.
    /// </summary>
    /// <value>Mensaje de error descriptivo. Campo obligatorio.</value>
    public required string Message { get; init; }
}
