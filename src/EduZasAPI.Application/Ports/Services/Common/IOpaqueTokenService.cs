namespace EduZasAPI.Application.Common;

/// <summary>
/// Define un contrato para servicios de generación de tokens opacos sin parámetros.
/// </summary>
/// <remarks>
/// Esta interfaz se utiliza para implementaciones que generan tokens opacos
/// (tokens que no contienen información legible y deben ser verificados contra
/// un almacenamiento) sin necesidad de argumentos adicionales.
/// </remarks>
public interface IOpaqueTokenService
{
    /// <summary>
    /// Genera un nuevo token opaco.
    /// </summary>
    /// <returns>Una cadena que representa el token opaco generado.</returns>
    /// <remarks>
    /// El token generado es opaco, lo que significa que no contiene información
    /// legible y debe ser almacenado y verificado contra un sistema de persistencia.
    /// </remarks>
    public string GenerateToken();
}

/// <summary>
/// Define un contrato para servicios de generación de tokens opacos con parámetros.
/// </summary>
/// <typeparam name="T">Tipo del argumento utilizado para generar el token.</typeparam>
/// <remarks>
/// Esta interfaz se utiliza para implementaciones que generan tokens opacos
/// utilizando información adicional específica del tipo T, como identificadores
/// de usuario, timestamps u otros datos relevantes para la generación del token.
/// </remarks>
public interface IOpaqueTokenService<T>
{
    /// <summary>
    /// Genera un nuevo token opaco utilizando el argumento proporcionado.
    /// </summary>
    /// <param name="arg">Argumento de tipo T utilizado en la generación del token.</param>
    /// <returns>Una cadena que representa el token opaco generado.</returns>
    /// <remarks>
    /// El token generado es opaco y puede incorporar información derivada del
    /// argumento proporcionado, pero sigue siendo necesario almacenarlo para
    /// su posterior verificación.
    /// </remarks>
    public string GenerateToken(T arg);
}
