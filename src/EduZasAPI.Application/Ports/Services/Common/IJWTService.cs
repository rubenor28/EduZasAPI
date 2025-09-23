using EduZasAPI.Domain.Common;

namespace EduZasAPI.Application.Common;

/// <summary>
/// Define un contrato para servicios de generación y validación de tokens JWT (JSON Web Tokens).
/// </summary>
/// <remarks>
/// Esta interfaz se utiliza para implementaciones que generan y validan tokens JWT,
/// que son tokens firmados que contienen información en formato JSON y están protegidos
/// mediante firma digital para garantizar su integridad y autenticidad.
/// </remarks>
public interface IJWTService
{
    /// <summary>
    /// Genera un nuevo token JWT con el payload especificado.
    /// </summary>
    /// <typeparam name="T">Tipo del payload a incluir en el token. Debe ser no nulo.</typeparam>
    /// <param name="secret">Secreto utilizado para firmar el token JWT.</param>
    /// <param name="expiration">Duración de expiración del token como TimeSpan.</param>
    /// <param name="payload">Datos a incluir en el payload del token JWT. No puede ser nulo.</param>
    /// <returns>Una cadena que representa el token JWT generado.</returns>
    /// <remarks>
    /// El token JWT generado contiene información legible en formato JSON en su payload
    /// y está firmado digitalmente para prevenir modificaciones.
    /// </remarks>
    string Generate<T>(string secret, TimeSpan expiration, T payload) where T : notnull;

    /// <summary>
    /// Valida la integridad, autenticidad y expiración de un token JWT.
    /// </summary>
    /// <typeparam name="T">Tipo esperado del payload del token JWT. Debe ser no nulo.</typeparam>
    /// <param name="secret">Secreto utilizado para verificar la firma del token JWT.</param>
    /// <param name="token">Token JWT a validar.</param>
    /// <returns>
    /// Un <see cref="Result{T, E}"/> que contiene el payload del token JWT decodificado si es válido,
    /// o un <see cref="SignedTokenErrors"/> que describe el error de validación.
    /// </returns>
    /// <remarks>
    /// Este método verifica que la firma del token JWT sea válida utilizando el secreto proporcionado,
    /// que el token no haya expirado y que esté correctamente formado según el estándar JWT.
    /// </remarks>
    Result<T, SignedTokenErrors> IsValid<T>(string secret, string token) where T : notnull;
}
