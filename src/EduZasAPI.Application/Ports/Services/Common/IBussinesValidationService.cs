namespace EduZasAPI.Application.Ports.Services.Common;

using EduZasAPI.Domain.ValueObjects.Common;
using EduZasAPI.Application.DTOs.Common;

/// <summary>
/// Define un contrato para servicios de validación de reglas de negocio.
/// </summary>
/// <typeparam name="T">Tipo de los datos a validar.</typeparam>
/// <remarks>
/// Esta interfaz se utiliza para implementar servicios que validan reglas de negocio
/// y devuelven un resultado que puede ser exitoso (Unit) o una colección de errores
/// de validación específicos por campo.
/// </remarks>
public interface IBusinessValidationService<T>
{
    /// <summary>
    /// Valida si los datos cumplen con las reglas de negocio especificadas.
    /// </summary>
    /// <param name="data">Datos a validar.</param>
    /// <returns>
    /// Un <see cref="Result{T, E}"/> que contiene <see cref="Unit"/> si los datos son válidos,
    /// o un arreglo de <see cref="FieldErrorDTO"/> con los errores de validación por campo.
    /// </returns>
    Result<Unit, List<FieldErrorDTO>> IsValid(T data);
}
