namespace EduZasAPI.Domain.Common;

/// <summary>
/// Enumera los tipos de errores que pueden ocurrir durante la validación de tokens firmados.
/// </summary>
public enum SignedTokenErrors
{
    /// <summary>
    /// Error desconocido o no especificado durante la validación del token.
    /// </summary>
    Unknown,

    /// <summary>
    /// El token ha expirado según su período de validez.
    /// </summary>
    TokenExpired,

    /// <summary>
    /// El token es inválido o está mal formado.
    /// </summary>
    TokenInvalid,
}
