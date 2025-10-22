using Application.DTOs.Common;
using Application.UseCases.Notifications;
using Domain.Entities;
using MinimalAPI.Application.DTOs.Common;
using MinimalAPI.Application.DTOs.Notifications;
using MinimalAPI.Presentation.Filters;
using MinimalAPI.Presentation.Mappers;

namespace MinimalAPI.Presentation.Routes;

public static class NotificationRoutes
{
    public static RouteGroupBuilder MapNotificationRoutes(this WebApplication app)
    {
        var group = app.MapGroup("/notifications").WithTags("Notifications");

        app.MapGet("/{page:int}", GetUserNotifications)
            .WithName("Obtener notificaciones por usuario")
            .AddEndpointFilter<UserIdFilter>()
            .RequireAuthorization("RequireAuthenticated")
            .Produces<PaginatedQuery<NotificationDomain, NotificationCriteriaMAPI>>(
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
        int page,
        SearchNotificationUseCase useCase,
        HttpContext ctx,
        RoutesUtils utils
    )
    {
        var userId = utils.GetIdFromContext(ctx);
        var search = await useCase.ExecuteAsync(
            new() { Page = page <= 0 ? 1 : page, UserId = userId }
        );

        var results = new
        {
            search.Page,
            search.Results,
            search.TotalPages,
            Criteria = search.Criteria.FromDomain(),
        };

        return Results.Ok(results);
    }
}
