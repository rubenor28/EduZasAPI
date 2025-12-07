using Application.DTOs.Common;
using Application.UseCases.Common;
using Domain.Enums;
using Domain.ValueObjects;
using MinimalAPI.Application.DTOs.Common;

namespace MinimalAPI.Presentation.Routes;

/// <summary>
/// Proporciona métodos de utilidad para encapsular la lógica de ejecución de rutas en Minimal API.
/// </summary>
/// <remarks>
/// Esta clase centraliza el manejo de excepciones, la validación de entradas y la orquestación
/// de casos de uso, asegurando que las rutas devuelvan respuestas HTTP consistentes y apropiadas.
/// La implementación actual registra las excepciones en la consola; se planea integrar un sistema de logging más robusto.
/// </remarks>
public class RoutesUtils
{
    public static UserType MapRole(string role) =>
        role switch
        {
            "ADMIN" => UserType.ADMIN,
            "PROFESSOR" => UserType.PROFESSOR,
            "STUDENT" => UserType.STUDENT,
            _ => throw new NotImplementedException(),
        };

    public static IResult MapError(UseCaseError err) =>
        err switch
        {
            InputError errs => Results.BadRequest(
                new FieldErrorResponse { Message = "Formato inválido", Errors = errs.Errors }
            ),
            UnauthorizedError => Results.Forbid(),
            NotFoundError => Results.NotFound(),
            Conflict c => Results.Conflict(new MessageResponse { Message = c.Message }),
            _ => throw new NotImplementedException(),
        };

    /// <summary>
    /// Ejecuta de forma segura una acción de ruta sincrónica, capturando cualquier excepción inesperada.
    /// </summary>
    /// <param name="routeAction">Función que representa la lógica de la ruta y devuelve un <see cref="IResult"/>.</param>
    /// <returns>
    /// El resultado de la acción de la ruta si la ejecución es exitosa,
    /// o un <see cref="Results.InternalServerError"/> si ocurre una excepción no controlada.
    /// </returns>
    public IResult HandleResponse(Func<IResult> routeAction)
    {
        try
        {
            return routeAction();
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error: {e.Message}");
            Console.WriteLine(e.StackTrace);
            return Results.InternalServerError();
        }
    }

    /// <summary>
    /// Ejecuta de forma segura una acción de ruta asincrónica, capturando cualquier excepción inesperada.
    /// </summary>
    /// <param name="routeAction">Función asincrónica que representa la lógica de la ruta y devuelve un <see cref="Task{IResult}"/>.</param>
    /// <returns>
    /// Una tarea que produce el resultado de la acción de la ruta si la ejecución es exitosa,
    /// o un <see cref="Results.InternalServerError"/> encapsulado en una tarea si ocurre una excepción no controlada.
    /// </returns>
    public async Task<IResult> HandleResponseAsync(Func<Task<IResult>> routeAction)
    {
        try
        {
            return await routeAction();
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error: {e.Message}");
            Console.WriteLine(e.StackTrace);
            return Results.InternalServerError();
        }
    }

    public async Task<IResult> HandleGuestUseCaseAsync<TRequest, TResponse>(
        IGuestUseCaseAsync<TRequest, TResponse> useCase,
        Func<TRequest> mapRequest,
        Func<TResponse, IResult> mapResponse
    )
        where TRequest : notnull
        where TResponse : notnull
    {
        return await HandleResponseAsync(async () =>
        {
            var request = mapRequest();

            var result = await useCase.ExecuteAsync(request);

            if (result.IsErr)
                return MapError(result.UnwrapErr());

            return mapResponse(result.Unwrap());
        });
    }

    /// <summary>
    /// Orquesta la ejecución de un caso de uso asincrónico, manejando el mapeo de la solicitud, la ejecución del caso de uso y el mapeo de la respuesta.
    /// </summary>
    /// <typeparam name="TRequest">El tipo del objeto de solicitud del caso de uso.</typeparam>
    /// <typeparam name="TResponse">El tipo del objeto de respuesta del caso de uso.</typeparam>
    /// <param name="useCase">La instancia del caso de uso a ejecutar.</param>
    /// <param name="mapRequest">Una función asincrónica que valida y mapea la entrada HTTP a un objeto de solicitud <typeparamref name="TRequest"/>.</param>
    /// <param name="mapResponse">Una función que mapea la respuesta exitosa del caso de uso <typeparamref name="TResponse"/> a un <see cref="IResult"/>.</param>
    /// <returns>
    /// Un <see cref="IResult"/> que representa el resultado de la operación. Puede ser una respuesta de éxito,
    /// un error de validación (BadRequest) o un error de lógica de negocio mapeado por <see cref="useCaseErrorMapper"/>.
    /// </returns>
    public async Task<IResult> HandleUseCaseAsync<TRequest, TResponse>(
        HttpContext ctx,
        IUseCaseAsync<TRequest, TResponse> useCase,
        Func<Result<TRequest, IEnumerable<FieldErrorDTO>>> mapRequest,
        Func<TResponse, IResult> mapResponse
    )
        where TRequest : notnull
        where TResponse : notnull
    {
        return await HandleResponseAsync(async () =>
        {
            var requestMap = mapRequest();
            if (requestMap.IsErr)
                return MapError(UseCaseErrors.Input(requestMap.UnwrapErr()));

            var result = await useCase.ExecuteAsync(
                new() { Data = requestMap.Unwrap(), Executor = GetExecutorFromContext(ctx) }
            );

            if (result.IsErr)
                return MapError(result.UnwrapErr());

            return mapResponse(result.Unwrap());
        });
    }

    /// <summary>
    /// Construye un objeto <see cref="Executor"/> a partir de la información del usuario autenticado en el contexto HTTP.
    /// </summary>
    /// <param name="ctx">El <see cref="HttpContext"/> de la solicitud actual.</param>
    /// <returns>Una instancia de <see cref="Executor"/> con el Id y el Rol del usuario.</returns>
    /// <exception cref="InvalidDataException">Se lanza si la información del usuario (Id o Rol) no se encuentra o es inválida.</exception>
    public Executor GetExecutorFromContext(HttpContext ctx)
    {
        var userId =
            (string?)ctx.Items["UserId"]
            ?? throw new InvalidDataException(
                "Error al procesar el Executor: el 'UserId' no se encontró en el contexto."
            );

        var userRole =
            (string?)ctx.Items["UserRole"]
            ?? throw new InvalidDataException(
                "Error al procesar el Executor: el 'UserRole' no se encontró en el contexto."
            );

        var roleParse = MapRole(userRole);

        return new Executor { Id = ulong.Parse(userId), Role = roleParse };
    }

    /// <summary>
    /// Convierte una colección de errores de campo en una respuesta HTTP 400 (Bad Request).
    /// </summary>
    /// <param name="errors">La colección de errores de campo.</param>
    /// <returns>Un <see cref="IResult"/> que representa una respuesta de tipo <see cref="Results.BadRequest"/>.</returns>
    public IResult FieldErrorToBadRequest(IEnumerable<FieldErrorDTO> errors)
    {
        var response = new FieldErrorResponse { Message = "Formato inválido", Errors = errors };
        return Results.BadRequest(response);
    }

    /// <summary>
    /// Convierte un resultado de validación fallido en una respuesta HTTP 400 (Bad Request).
    /// </summary>
    /// <typeparam name="T">El tipo del valor exitoso del resultado, no utilizado en caso de error.</typeparam>
    /// <param name="validation">El resultado de una operación de validación.</param>
    /// <returns>
    /// Un <see cref="IResult"/> que representa una respuesta de tipo <see cref="Results.BadRequest"/>
    /// si la validación contiene errores.
    /// </returns>
    public IResult FieldErrorToBadRequest<T>(Result<T, IEnumerable<FieldErrorDTO>> validation)
        where T : notnull
    {
        var errors = validation.UnwrapErr();
        return FieldErrorToBadRequest(errors);
    }
}
