using Application.DTOs.Common;
using Application.DTOs.Contacts;
using Application.DTOs.ContactTags;
using Application.DTOs.Tags;
using Application.UseCases.Contacts;
using Application.UseCases.ContactTags;
using Application.UseCases.Tags;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using MinimalAPI.Application.DTOs.Common;
using MinimalAPI.Presentation.Filters;

namespace MinimalAPI.Presentation.Routes;

/// <summary>
/// Define las rutas relacionadas con la gestión de contactos.
/// </summary>
public static class ContactRoutes
{
    /// <summary>
    /// Mapea los endpoints para la gestión de contactos.
    /// </summary>
    /// <param name="app">La aplicación web.</param>
    /// <returns>El grupo de rutas configurado.</returns>
    public static RouteGroupBuilder MapContactRoutes(this WebApplication app)
    {
        var group = app.MapGroup("/contacts").WithTags("Contactos");

        group
            .MapPost("/", AddContact)
            .RequireAuthorization("ProfessorOrAdmin")
            .AddEndpointFilter<ExecutorFilter>()
            .Produces<ContactDomain>(StatusCodes.Status201Created)
            .Produces<FieldErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status409Conflict)
            .WithOpenApi(op =>
            {
                op.Summary = "Agregar un nuevo contacto a una agenda.";
                op.Description =
                    "Crea una relación de contacto entre el dueño de la agenda y otro usuario.";
                op.Responses["201"].Description = "Contacto creado exitosamente.";
                op.Responses["400"].Description =
                    "El ID del dueño de la agenda o del usuario a contactar no es válido.";
                op.Responses["401"].Description = "Usuario no autenticado.";
                op.Responses["403"].Description =
                    "El usuario no tiene permisos para modificar la agenda especificada.";
                op.Responses["409"].Description = "El contacto ya existe en la agenda.";
                return op;
            });

        group
            .MapPost("/me", SearchMyContacts)
            .RequireAuthorization("ProfessorOrAdmin")
            .AddEndpointFilter<ExecutorFilter>()
            .Produces<PaginatedQuery<ContactDomain, ContactCriteriaDTO>>(StatusCodes.Status200OK)
            .Produces<FieldErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .WithOpenApi(op =>
            {
                op.Summary = "Buscar en los contactos del usuario actual.";
                op.Description =
                    "Obtiene una lista paginada de los contactos del usuario que realiza la solicitud.";
                op.Responses["200"].Description = "Búsqueda completada exitosamente.";
                op.Responses["400"].Description = "Los criterios de búsqueda son inválidos.";
                op.Responses["401"].Description = "Usuario no autenticado.";
                op.Responses["403"].Description =
                    "El usuario no tiene permisos para ver contactos.";
                return op;
            });

        group
            .MapPost("/all", SearchContacts)
            .RequireAuthorization("Admin")
            .AddEndpointFilter<ExecutorFilter>()
            .Produces<PaginatedQuery<ContactDomain, ContactCriteriaDTO>>(StatusCodes.Status200OK)
            .Produces<FieldErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .WithOpenApi(op =>
            {
                op.Summary = "Buscar en todos los contactos (Solo Admin).";
                op.Description =
                    "Obtiene una lista paginada de todos los contactos del sistema, con filtros.";
                op.Responses["200"].Description = "Búsqueda completada exitosamente.";
                op.Responses["400"].Description = "Los criterios de búsqueda son inválidos.";
                op.Responses["401"].Description = "Usuario no autenticado.";
                op.Responses["403"].Description = "El usuario no es administrador.";
                return op;
            });

        group
            .MapDelete("/{agendaOwnerId:ulong}/{contactId:ulong}", DeleteContact)
            .RequireAuthorization("ProfessorOrAdmin")
            .AddEndpointFilter<ExecutorFilter>()
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .WithOpenApi(op =>
            {
                op.Summary = "Eliminar un contacto.";
                op.Description = "Elimina un contacto de una agenda específica.";
                op.Responses["204"].Description = "Contacto eliminado exitosamente.";
                op.Responses["401"].Description = "Usuario no autenticado.";
                op.Responses["403"].Description =
                    "El usuario no tiene permisos para modificar la agenda.";
                op.Responses["404"].Description = "El contacto no fue encontrado en la agenda.";
                return op;
            });

        group
            .MapPut("/", UpdateContact)
            .RequireAuthorization("ProfessorOrAdmin")
            .AddEndpointFilter<ExecutorFilter>()
            .Produces<ContactDomain>(StatusCodes.Status200OK)
            .Produces<FieldErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .WithOpenApi(op =>
            {
                op.Summary = "Actualizar un contacto.";
                op.Description = "Modifica la información de un contacto existente.";
                op.Responses["200"].Description = "Contacto actualizado exitosamente.";
                op.Responses["400"].Description = "Los datos para la actualización son inválidos.";
                op.Responses["401"].Description = "Usuario no autenticado.";
                op.Responses["403"].Description =
                    "El usuario no tiene permisos para modificar el contacto.";
                op.Responses["404"].Description = "El contacto no fue encontrado.";
                return op;
            });

        group
            .MapPost("/tags/search", TagsQuery)
            .RequireAuthorization("ProfessorOrAdmin")
            .AddEndpointFilter<ExecutorFilter>()
            .Produces<PaginatedQuery<string, TagCriteriaDTO>>(StatusCodes.Status200OK)
            .Produces<FieldErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .WithOpenApi(op =>
            {
                op.Summary = "Buscar etiquetas de contacto.";
                op.Description =
                    "Obtiene una lista paginada de etiquetas según criterios de búsqueda.";
                op.Responses["200"].Description = "Búsqueda completada exitosamente.";
                op.Responses["400"].Description = "Criterios de búsqueda inválidos.";
                op.Responses["401"].Description = "Usuario no autenticado.";
                op.Responses["403"].Description =
                    "El usuario no tiene permisos para buscar etiquetas.";
                return op;
            });

        group
            .MapPost("/tags", AddContactTag)
            .RequireAuthorization("ProfessorOrAdmin")
            .AddEndpointFilter<ExecutorFilter>()
            .Produces(StatusCodes.Status204NoContent)
            .Produces<FieldErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status409Conflict)
            .WithOpenApi(op =>
            {
                op.Summary = "Asignar una etiqueta a un contacto.";
                op.Description =
                    "Crea una asociación entre un contacto y una etiqueta. Si la etiqueta no existe, se crea.";
                op.Responses["204"].Description = "Etiqueta asignada exitosamente.";
                op.Responses["400"].Description = "IDs de usuario inválidos.";
                op.Responses["401"].Description = "Usuario no autenticado.";
                op.Responses["403"].Description =
                    "El usuario no tiene permisos para etiquetar en esa agenda.";
                op.Responses["404"].Description = "El contacto especificado no existe.";
                op.Responses["409"].Description = "La etiqueta ya está asignada a este contacto.";
                return op;
            });

        group
            .MapDelete("/tags/{agendaOwnerId:ulong}/{userId:ulong}/{tag}", DeleteContactTag)
            .RequireAuthorization("ProfessorOrAdmin")
            .AddEndpointFilter<ExecutorFilter>()
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .WithOpenApi(op =>
            {
                op.Summary = "Quitar una etiqueta de un contacto.";
                op.Description = "Elimina la asociación entre un contacto y una etiqueta.";
                op.Responses["204"].Description = "Etiqueta eliminada del contacto exitosamente.";
                op.Responses["401"].Description = "Usuario no autenticado.";
                op.Responses["403"].Description =
                    "El usuario no tiene permisos para modificar la agenda.";
                op.Responses["404"].Description =
                    "La asociación entre el contacto y la etiqueta no fue encontrada.";
                return op;
            });

        return group;
    }

    private static Task<IResult> AddContact(
        [FromBody] NewContactDTO request,
        [FromServices] AddContactUseCase useCase,
        HttpContext ctx,
        [FromServices] RoutesUtils utils
    )
    {
        return utils.HandleUseCaseAsync(
            ctx,
            useCase,
            mapRequest: () => request,
            mapResponse: c => Results.Created($"/contacts/{c.Id}", c)
        );
    }

    private static Task<IResult> SearchMyContacts(
        [FromBody] ContactCriteriaDTO criteria,
        [FromServices] ContactQueryUseCase useCase,
        [FromServices] RoutesUtils utils,
        HttpContext ctx
    )
    {
        return utils.HandleUseCaseAsync(
            ctx,
            useCase,
            mapRequest: () =>
                criteria with
                {
                    AgendaOwnerId = utils.GetExecutorFromContext(ctx).Id,
                },
            mapResponse: s => Results.Ok(s)
        );
    }

    private static Task<IResult> SearchContacts(
        [FromBody] ContactCriteriaDTO criteria,
        [FromServices] ContactQueryUseCase useCase,
        HttpContext ctx,
        [FromServices] RoutesUtils utils
    )
    {
        return utils.HandleUseCaseAsync(
            ctx,
            useCase,
            mapRequest: () => criteria,
            mapResponse: search => Results.Ok(search)
        );
    }

    private static Task<IResult> DeleteContact(
        [FromRoute] ulong agendaOwnerId,
        [FromRoute] ulong contactId,
        [FromServices] DeleteContactUseCase useCase,
        HttpContext ctx,
        [FromServices] RoutesUtils utils
    )
    {
        return utils.HandleUseCaseAsync(
            ctx,
            useCase,
            mapRequest: () =>
                new ContactIdDTO { AgendaOwnerId = agendaOwnerId, UserId = contactId },
            mapResponse: (contact) => Results.Ok(contact)
        );
    }

    private static Task<IResult> UpdateContact(
        [FromBody] ContactUpdateDTO request,
        [FromServices] UpdateContactUseCase useCase,
        HttpContext ctx,
        [FromServices] RoutesUtils utils
    )
    {
        return utils.HandleUseCaseAsync(
            ctx,
            useCase,
            mapRequest: () => request,
            mapResponse: (contact) => Results.Ok(contact)
        );
    }

    private static Task<IResult> TagsQuery(
        [FromBody] TagCriteriaDTO criteria,
        [FromServices] TagQueryUseCase useCase,
        [FromServices] RoutesUtils utils,
        HttpContext ctx
    )
    {
        return utils.HandleUseCaseAsync(
            ctx,
            useCase,
            mapRequest: () => criteria,
            mapResponse: (search) => Results.Ok(search)
        );
    }

    private static Task<IResult> AddContactTag(
        [FromBody] ContactTagIdDTO value,
        [FromServices] AddContactTagUseCase useCase,
        [FromServices] RoutesUtils utils,
        HttpContext ctx
    )
    {
        return utils.HandleUseCaseAsync(
            ctx,
            useCase,
            mapRequest: () =>
                new NewContactTagDTO
                {
                    UserId = value.UserId,
                    Tag = value.Tag,
                    AgendaOwnerId = value.AgendaOwnerId,
                },
            mapResponse: (_) => Results.NoContent()
        );
    }

    private static Task<IResult> DeleteContactTag(
        [FromRoute] ulong agendaOwnerId,
        [FromRoute] ulong userId,
        [FromRoute] string tag,
        [FromServices] DeleteContactTagUseCase useCase,
        HttpContext ctx,
        [FromServices] RoutesUtils utils
    )
    {
        return utils.HandleUseCaseAsync(
            ctx,
            useCase,
            mapRequest: () =>
                new ContactTagIdDTO
                {
                    AgendaOwnerId = agendaOwnerId,
                    UserId = userId,
                    Tag = tag,
                },
            mapResponse: _ => Results.NoContent()
        );
    }
}
