namespace EduZasAPI.Application.Ports.Services.Common;

/// <summary>
/// Define un contrato para servicios de validación de reglas de negocio.
/// </summary>
/// <typeparam name="T">Tipo de los datos a validar.</typeparam>
/// <remarks>
/// Esta interfaz se utiliza para implementar servicios que validan reglas de negocio,
/// como validaciones de formato, reglas simples.
/// </remarks>
public interface IBusinessValidationService<T>
{
    /// <summary>
    /// Valida si los datos cumplen con las reglas de negocio especificadas.
    /// </summary>
    /// <param name="data">Datos a validar.</param>
    /// <returns>
    /// true si los datos son válidos según las reglas de negocio, false en caso contrario.
    /// </returns>
    bool IsValid(T data);
}
