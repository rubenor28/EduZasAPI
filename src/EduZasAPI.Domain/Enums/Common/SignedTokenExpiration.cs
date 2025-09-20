namespace EduZasAPI.Domain.Enums.Common;

/// <summary>
/// Enumera los períodos de expiración disponibles para tokens firmados.
/// </summary>
public enum SignedTokenExpiration
{
    /// <summary>
    /// Token expira en 15 minutos.
    /// </summary>
    Minutes15,

    /// <summary>
    /// Token expira en 30 minutos.
    /// </summary>
    Minutes30,

    /// <summary>
    /// Token expira en 1 hora.
    /// </summary>
    Hours1,

    /// <summary>
    /// Token expira en 24 horas.
    /// </summary>
    Hours24
}
