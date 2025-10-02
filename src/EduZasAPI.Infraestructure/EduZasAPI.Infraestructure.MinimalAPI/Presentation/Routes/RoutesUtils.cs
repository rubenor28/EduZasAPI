namespace EduZasAPI.Infraestructure.MinimalAPI.Presentation.Common;

/// <summary>
/// Utilidades para manejar la ejecución de rutas en Minimal API,
/// encapsulando la lógica de captura de excepciones y devolviendo
/// respuestas HTTP apropiadas en caso de error.
/// </summary>
public class RoutesUtils
{
    // TODO: (Tal vez) Agregar un repositorio o algo similar para
    // almacenar los logs en algun lugar en caso de excepcion

    /// <summary>
    /// Ejecuta de forma segura una acción de ruta sincrónica.
    /// </summary>
    /// <param name="routeAction">Función que representa la lógica de la ruta y devuelve un <see cref="IResult"/>.</param>
    /// <returns>
    /// El resultado de la acción de la ruta si la ejecución es exitosa,
    /// o un <see cref="Results.InternalServerError"/> si ocurre una excepción.
    /// </returns>
    public IResult HandleResponse(Func<IResult> routeAction)
    {
        try
        {
            return routeAction();
        }
        catch (System.Exception e)
        {
            Console.WriteLine($"Error: {e.Message}");
            Console.WriteLine(e.StackTrace);
            return Results.InternalServerError();
        }
    }

    /// <summary>
    /// Ejecuta de forma segura una acción de ruta asincrónica.
    /// </summary>
    /// <param name="routeAction">Función asincrónica que representa la lógica de la ruta y devuelve un <see cref="Task{IResult}"/>.</param>
    /// <returns>
    /// Una tarea que produce el resultado de la acción de la ruta si la ejecución es exitosa,
    /// o un <see cref="Results.InternalServerError"/> encapsulado en una tarea si ocurre una excepción.
    /// </returns>
    public Task<IResult> HandleResponseAsync(Func<Task<IResult>> routeAction)
    {
        try
        {
            return routeAction();
        }
        catch (System.Exception e)
        {
            Console.WriteLine($"Error: {e.Message}");
            Console.WriteLine(e.StackTrace);
            return Task.FromResult(Results.InternalServerError());
        }
    }

    public ulong GetIdFromContext(HttpContext ctx)
    {
        var userId = (string?)ctx.Items["UserId"];
        if (userId is null) throw new InvalidDataException("Error al procesar el usuario");
        return ulong.Parse(userId);
    }
}
