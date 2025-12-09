using Application.DTOs.Common;
using Application.UseCases.Database;
using Domain.ValueObjects;
using InterfaceAdapters.Mappers.Common;
using Microsoft.AspNetCore.Mvc;
using MinimalAPI.Presentation.Filters;

namespace MinimalAPI.Presentation.Routes;

/// <summary>
/// Define las rutas para operaciones de base de datos (respaldo y restauración).
/// </summary>
public static class DatabaseRoutes
{
    /// <summary>
    /// Mapea los endpoints para operaciones de base de datos.
    /// </summary>
    /// <param name="app">La aplicación web.</param>
    /// <returns>El grupo de rutas configurado.</returns>
    public static RouteGroupBuilder MapDatabaseRoutes(this WebApplication app)
    {
        var group = app.MapGroup("/database").WithTags("Respaldo y Restauracion");

        group
            .MapGet("/backup", CreateBackup)
            .RequireAuthorization("Admin")
            .AddEndpointFilter<ExecutorFilter>()
            .Produces<FileStreamResult>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .WithOpenApi(op =>
            {
                op.Summary = "Crear un respaldo de la base de datos.";
                op.Description =
                    "Genera un archivo .sql con el estado actual de la base de datos. Requiere privilegios de administrador.";
                op.Responses["200"].Description =
                    "Respaldo generado exitosamente. El cuerpo de la respuesta es el archivo .sql.";
                op.Responses["401"].Description = "Usuario no autenticado.";
                op.Responses["403"].Description = "El usuario no es administrador.";
                return op;
            });

        group
            .MapPost("/restore", RestoreFromBackup)
            .RequireAuthorization("Admin")
            .AddEndpointFilter<ExecutorFilter>()
            .Accepts<IFormFile>("multipart/form-data")
            .Produces<string>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .WithOpenApi(op =>
            {
                op.Summary = "Restaurar la base de datos desde un respaldo.";
                op.Description =
                    "Carga un archivo .sql para restaurar el estado de la base de datos. Esta operación es destructiva. Requiere privilegios de administrador.";
                op.Responses["200"].Description = "Restauración completada con éxito.";
                op.Responses["400"].Description =
                    "No se proporcionó un archivo o el archivo está vacío.";
                op.Responses["401"].Description = "Usuario no autenticado.";
                op.Responses["403"].Description = "El usuario no es administrador.";
                return op;
            });

        return group;
    }

    public static async Task<IResult> CreateBackup(
        [FromServices] BackupUseCase useCase,
        [FromServices] RoutesUtils utils,
        HttpContext ctx
    )
    {
        return await utils.HandleUseCaseAsync(
            ctx,
            useCase,
            mapRequest: () => Unit.Value,
            mapResponse: (stream) =>
            {
                var fileName = $"backup-{DateTime.UtcNow:yyyyMMddHHmmss}.sql";
                return Results.File(stream, "application/octet-stream", fileName);
            }
        );
    }

    public static async Task<IResult> RestoreFromBackup(
        [FromServices] RestoreUseCase useCase,
        [FromServices] RoutesUtils utils,
        [FromServices] IMapper<UseCaseError, IResult> useCaseErrorsMapper,
        HttpContext httpContext,
        IFormFile file
    )
    {
        return await utils.HandleResponseAsync(async () =>
        {
            if (file is null || file.Length == 0)
            {
                return Results.BadRequest(
                    "No se ha proporcionado un archivo o el archivo está vacío."
                );
            }

            var executor = utils.GetExecutorFromContext(httpContext);
            await using var inputStream = file.OpenReadStream();

            var result = await useCase.ExecuteAsync(
                new() { Executor = executor, Data = inputStream }
            );

            if (result.IsErr)
                return useCaseErrorsMapper.Map(result.UnwrapErr());

            return Results.Ok("Restauración completada con éxito.");
        });
    }
}
