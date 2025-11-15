namespace MinimalAPI.Presentation.Constraints;

public class UlongRouteConstraint : IRouteConstraint
{
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
