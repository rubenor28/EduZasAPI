using Domain.ValueObjects;

namespace Application.Services.Validators;

/// <summary>
/// Contrato para servicios de validación de reglas de negocio.
/// </summary>
/// <typeparam name="T">Tipo de datos a validar.</typeparam>
public interface IBusinessValidationService<T>
{
    /// <summary>
    /// Valida si los datos cumplen las reglas de negocio.
    /// </summary>
    /// <param name="data">Datos a validar.</param>
    /// <returns>Éxito (Unit) o lista de errores por campo.</returns>
    Result<Unit, IEnumerable<FieldErrorDTO>> IsValid(T data);
}
