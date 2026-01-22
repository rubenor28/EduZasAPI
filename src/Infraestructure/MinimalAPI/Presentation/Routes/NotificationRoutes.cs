using Domain.ValueObjects;
using Application.DTOs.Notifications;
using Application.DTOs.UserNotifications;
using Application.UseCases.Notifications;
using Application.UseCases.UserNotifications;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using MinimalAPI.Application.DTOs;
using MinimalAPI.Presentation.Filters;

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
            .MapPost("/", GetUserNotifications)
            .WithName("Obtener notificaciones por usuario")
            .AddEndpointFilter<ExecutorFilter>()
            .RequireAuthorization("RequireAuthenticated")
            .Produces<PaginatedQuery<NotificationSummaryDTO, NotificationSummaryCriteriaDTO>>(
                StatusCodes.Status200OK
            )
            .Produces<FieldErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .WithOpenApi(op =>
            {
                op.Summary = "Obtener notificaciones del usuario autenticado.";
                op.Description =
                    "Recupera una lista paginada de las notificaciones para el usuario que realiza la solicitud.";
                op.Responses["200"].Description =
                    "La lista de notificaciones fue recuperada exitosamente.";
                op.Responses["400"].Description = "Si el número de página es inválido.";
                op.Responses["401"].Description = "Si el usuario no está autenticado.";
                op.Responses["403"].Description = "Si el no tiene los permisos adecuados.";

                return op;
            });

        group
            .MapGet("/unreaded/{userId:ulong}", HasUnreadNotificationUseCase)
            .AddEndpointFilter<ExecutorFilter>()
            .RequireAuthorization("RequireAuthenticated")
            .Produces<bool>(StatusCodes.Status200OK);

        group
            .MapPut("/read", MarkNotificationAsRead)
            .AddEndpointFilter<ExecutorFilter>()
            .RequireAuthorization("RequireAuthenticated")
            .Produces<bool>(StatusCodes.Status204NoContent);

        return group;
    }

    public static async Task<IResult> GetUserNotifications(
        [FromBody] NotificationSummaryCriteriaDTO criteria,
        [FromServices] QueryNotificationSummaryUseCase useCase,
        HttpContext ctx,
        [FromServices] RoutesUtils utils
    )
    {
        return await utils.HandleUseCaseAsync(
            ctx,
            useCase,
            mapRequest: () => criteria,
            mapResponse: search => Results.Ok(search)
        );
    }

    public static Task<IResult> MarkNotificationAsRead(
        [FromBody] UserNotificationIdDTO request,
        [FromServices] UpdateUserNotificationUseCase useCase,
        [FromServices] RoutesUtils utils,
        HttpContext ctx
    ) =>
        utils.HandleUseCaseAsync(
            ctx,
            useCase,
            mapRequest: () =>
                new UserNotificationUpdateDTO
                {
                    NotificationId = request.NotificationId,
                    UserId = request.UserId,
                    Readed = true,
                },
            mapResponse: _ => Results.NoContent()
        );

    public static Task<IResult> HasUnreadNotificationUseCase(
        [FromRoute] ulong userId,
        [FromServices] HasUnreadNotificationUseCase useCase,
        HttpContext ctx,
        [FromServices] RoutesUtils utils
    ) =>
        utils.HandleUseCaseAsync(
            ctx,
            useCase,
            mapRequest: () => userId,
            mapResponse: res => Results.Ok(res)
        );
}
