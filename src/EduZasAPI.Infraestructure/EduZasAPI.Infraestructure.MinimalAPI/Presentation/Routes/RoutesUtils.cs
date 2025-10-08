using EduZasAPI.Domain.Common;
using EduZasAPI.Domain.Users;
using EduZasAPI.Application.Common;
using EduZasAPI.Application.Users;
using EduZasAPI.Infraestructure.MinimalAPI.Application.Common;

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

    public Executor GetExecutorFromContext(HttpContext ctx)
    {
        var userId = (string?)ctx.Items["UserId"];
        var userRole = (string?)ctx.Items["UserRole"];

        if (userId is null) throw new InvalidDataException("Error al procesar el Executor");
        if (userRole is null) throw new InvalidDataException("Error al procesar el Executor");

        var roleParse = UserTypeMapper.FromString(userRole);

        if (roleParse.IsNone) throw new InvalidDataException("Error al procesar el Executor");

        return new Executor
        {
            Id = ulong.Parse(userId),
            Role = roleParse.Unwrap()
        };
    }

    public IResult FieldErrorToBadRequest(List<FieldErrorDTO> errors)
    {
        var response = new FieldErrorResponse { Message = "Formato inválido", Errors = errors };
        return Results.BadRequest(response);
    }

    public IResult FieldErrorToBadRequest<T>(Result<T, List<FieldErrorDTO>> validation) where T : notnull
    {
        var errors = validation.UnwrapErr();
        return FieldErrorToBadRequest(errors);
    }
}
