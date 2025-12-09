namespace MinimalAPI.Presentation.Constraints;

/// <summary>
/// Restricción de ruta que valida si un parámetro es un número entero sin signo de 64 bits (ulong).
/// </summary>
public class UlongRouteConstraint : IRouteConstraint
{
    /// <summary>
    /// Determina si el valor de la ruta cumple con la restricción.
    /// </summary>
    /// <param name="httpContext">Contexto HTTP.</param>
    /// <param name="route">La ruta que se está evaluando.</param>
    /// <param name="routeKey">La clave del parámetro de ruta.</param>
    /// <param name="values">Diccionario de valores de ruta.</param>
    /// <param name="routeDirection">Dirección del enrutamiento.</param>
    /// <returns>True si el valor es un ulong válido; de lo contrario, false.</returns>
    public bool Match(
        HttpContext? httpContext,
        IRouter? route,
        string routeKey,
        RouteValueDictionary values,
        RouteDirection routeDirection
    )
    {
        if (values.TryGetValue(routeKey, out var value) && value != null)
        {
            return ulong.TryParse(value.ToString(), out _);
        }

        return false;
    }
}
