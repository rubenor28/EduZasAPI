using Application.DTOs.Common;
using Application.DTOs.Database;
using Application.UseCases.Database;
using InterfaceAdapters.Mappers.Common;
using MinimalAPI.Presentation.Mappers;

namespace MinimalAPI.Presentation.Routes;

public static class DatabaseRoutes
{
    public static RouteGroupBuilder MapDatabaseRoutes(this WebApplication app)
    {
        var group = app.MapGroup("/database").WithTags("Respaldo y Restauracion");

        group
            .MapGet("/backup", CreateBackup)
            .WithName("Crear respaldo")
            .RequireAuthorization("Admin");

        group
            .MapPost("/restore", RestoreFromBackup)
            .WithName("Restaurar desde respaldo")
            .RequireAuthorization("Admin")
            .Accepts<IFormFile>("multipart/form-data");

        return group;
    }

    public static async Task<IResult> CreateBackup(
        BackupUseCase useCase,
        RoutesUtils utils,
        HttpContext ctx
    )
    {
        return await utils.HandleUseCaseAsync(
            useCase,
            mapRequest: () => utils.GetExecutorFromContext(ctx),
            mapResponse: (stream) =>
            {
                var fileName = $"backup-{DateTime.UtcNow:yyyyMMddHHmmss}.sql";
                return Results.File(stream, "application/octet-stream", fileName);
            }
        );
    }

    public static async Task<IResult> RestoreFromBackup(
        RestoreUseCase useCase,
        RoutesUtils utils,
        HttpContext httpContext,
        IFormFile file,
        IMapper<UseCaseError, IResult> useCaseErrorsMapper
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

            var request = new RestoreRequestDTO { Executor = executor, InputStream = inputStream };
            var result = await useCase.ExecuteAsync(request);

            if (result.IsErr)
                return useCaseErrorsMapper.Map(result.UnwrapErr());

            return Results.Ok("Restauración completada con éxito.");
        });
    }
}
