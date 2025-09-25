using EduZasAPI.Domain.Common;

namespace EduZasAPI.Application.Common;

/// <summary>
/// Define un contrato para servicios de generación y validación de tokens firmados.
/// </summary>
/// <remarks>
/// Esta interfaz se utiliza para implementaciones que generan y validan tokens firmados
/// que contienen datos personalizados y están protegidos mediante firma digital
/// para garantizar su integridad y autenticidad.
/// </remarks>
public interface ISignedTokenService
{
    /// <summary>
    /// Genera un nuevo token firmado con los datos personalizados especificados.
    /// </summary>
    /// <typeparam name="T">Tipo de los datos personalizados a incluir en el token. Debe ser no nulo.</typeparam>
    /// <param name="customData">Datos personalizados a incluir en el token. No puede ser nulo.</param>
    /// <returns>Una cadena que representa el token firmado generado.</returns>
    /// <remarks>
    /// El token generado contiene los datos personalizados en formato legible y está
    /// firmado digitalmente para prevenir modificaciones no autorizadas.
    /// </remarks>
    string Generate<T>(T customData) where T : notnull;

    /// <summary>
    /// Valida la integridad y autenticidad de un token firmado.
    /// </summary>
    /// <typeparam name="T">Tipo esperado de los datos contenidos en el token. Debe ser no nulo.</typeparam>
    /// <param name="secret">Secreto utilizado para verificar la firma del token.</param>
    /// <param name="token">Token firmado a validar.</param>
    /// <returns>
    /// Un <see cref="Result{T, E}"/> que contiene los datos decodificados del token si es válido,
    /// o un <see cref="SignedTokenErrors"/> que describe el error de validación.
    /// </returns>
    /// <remarks>
    /// Este método verifica que la firma del token sea válida utilizando el secreto proporcionado
    /// y que el token no haya expirado o esté mal formado.
    /// </remarks>
    Result<T, SignedTokenErrors> IsValid<T>(string secret, string token) where T : notnull;
}
