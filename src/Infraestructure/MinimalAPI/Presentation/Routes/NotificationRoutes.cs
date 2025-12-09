using Application.DAOs;
using Application.DTOs.Common;
using Application.DTOs.Notifications;
using Application.UseCases.Notifications;
using Domain.Entities;
using MinimalAPI.Application.DTOs.Common;
using MinimalAPI.Presentation.Filters;
using Microsoft.AspNetCore.Mvc;

namespace MinimalAPI.Presentation.Routes;

/// <summary>
/// Define las rutas relacionadas con las notificaciones.
/// </summary>
public static class NotificationRoutes
{
    /// <summary>
    /// Mapea los endpoints para la gestión de notificaciones.
    /// </summary>
    /// <param name="app">La aplicación web.</param>
    /// <returns>El grupo de rutas configurado.</returns>
    public static RouteGroupBuilder MapNotificationRoutes(this WebApplication app)
    {
        var group = app.MapGroup("/notifications").WithTags("Notifications");

        group
            .MapGet("/{page:int}", GetUserNotifications)
            .WithName("Obtener notificaciones por usuario")
            .AddEndpointFilter<ExecutorFilter>()
            .RequireAuthorization("RequireAuthenticated")
            .Produces<PaginatedQuery<NotificationDomain, NotificationCriteriaDTO>>(
                StatusCodes.Status200OK
            )
            .Produces<FieldErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .WithOpenApi(op =>
            {
                op.Summary = "Obtener notificaciones del usuario autenticado.";
                op.Description =
                    "Recupera una lista paginada de las notificaciones para el usuario que realiza la solicitud.";
                op.Responses["200"].Description =
                    "La lista de notificaciones fue recuperada exitosamente.";
                op.Responses["400"].Description = "Si el número de página es inválido.";
                op.Responses["401"].Description = "Si el usuario no está autenticado.";
                return op;
            });

        return group;
    }

    public static async Task<IResult> GetUserNotifications(
        [FromRoute] int page,
        [FromServices] SearchNotificationUseCase useCase,
        HttpContext ctx,
        [FromServices] RoutesUtils utils,
        [FromServices] IReaderAsync<string, ClassDomain> classReader
    )
    {
        return await utils.HandleUseCaseAsync(
            ctx,
            useCase,
            mapRequest: () =>
                new NotificationCriteriaDTO
                {
                    Page = page,
                    UserId = utils.GetExecutorFromContext(ctx).Id,
                },
            mapResponse: search => Results.Ok(search)
        );
    }
}
