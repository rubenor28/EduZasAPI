using Application.DTOs.Common;
using Application.DTOs.Contacts;
using Application.UseCases.Common;
using Application.UseCases.Contacts;
using Application.UseCases.ContactTags;
using Application.UseCases.Tags;
using Domain.Entities;
using Domain.ValueObjects;
using MinimalAPI.Application.DTOs.Contacts;
using MinimalAPI.Application.DTOs.Tags;
using MinimalAPI.Presentation.Filters;
using MinimalAPI.Presentation.Mappers;

namespace MinimalAPI.Presentation.Routes;

public static class ContactRoutes
{
    public static RouteGroupBuilder MappContactRoutes(this WebApplication app)
    {
        var group = app.MapGroup("/contacts").WithTags("Contactos");

        app.MapPost("/", AddContact)
            .WithName("Agregar contacto")
            .RequireAuthorization("ProfesorOrAdmin")
            .AddEndpointFilter<ExecutorFilter>();

        app.MapPost("/me", SearchMyContacts)
            .WithName("Buscar mis contactos")
            .RequireAuthorization("ProfesorOrAdmin")
            .AddEndpointFilter<UserIdFilter>();

        app.MapPost("/all", SearchContacts)
            .WithName("Buscar contactos")
            .RequireAuthorization("Admin")
            .AddEndpointFilter<ExecutorFilter>();

        app.MapDelete("/{agendaOwnerId:ulong}/{contactId:ulong}", DeleteContact)
            .WithName("Eliminar contacto")
            .RequireAuthorization("ProfesorOrAdmin")
            .AddEndpointFilter<ExecutorFilter>();

        app.MapPut("/", UpdateContact)
            .WithName("Actualizar contacto")
            .RequireAuthorization("ProfesorOrAdmin")
            .AddEndpointFilter<ExecutorFilter>();

        app.MapPost("/tags/search", TagsQuery)
            .WithName("Obtener etiquetas de contacto")
            .RequireAuthorization("ProfesorOrAdmin")
            .AddEndpointFilter<ExecutorFilter>();

        app.MapPost("/", AddContactTag)
          .WithName("Agregar etiqueta a contacto")
          .RequireAuthorization("ProfesorOrAdmin")
          .AddEndpointFilter<ExecutorFilter>();

        app.MapDelete("/{agendaOwnerId:ulong}/users/{userId:ulong}/tags/{tag}", DeleteContactTag)
            .WithName("Eliminar etiqueta a contacto")
            .RequireAuthorization("ProfesorOrAdmin")
            .AddEndpointFilter<ExecutorFilter>();

        return group;
    }

    private static Task<IResult> AddContact(
        NewContactMAPI request,
        AddContactUseCase useCase,
        HttpContext ctx,
        RoutesUtils utils
    )
    {
        return utils.HandleResponseAsync(async () =>
        {
            var executor = utils.GetExecutorFromContext(ctx);
            var result = await useCase.ExecuteAsync(
                new()
                {
                    Alias = request.Alias,
                    Notes = request.Notes.ToOptional(),
                    AgendaOwnerId = request.AgendaOwnerId,
                    UserId = request.UserId,
                    Tags = request.Tags.ToOptional(),
                    Executor = executor,
                }
            );

            if (result.IsErr)
                return result.UnwrapErr().FromDomain();

            var contact = result.Unwrap();
            return Results.Created($"/contacts/{contact.Id}", contact.FromDomain());
        });
    }

    private static Task<IResult> SearchMyContacts(
        ContactCriteriaMAPI inputCriteria,
        ContactQueryUseCase useCase,
        HttpContext ctx,
        RoutesUtils utils
    )
    {
        return utils.HandleResponseAsync(async () =>
        {
            var userId = utils.GetIdFromContext(ctx);
            var criteria = inputCriteria with { AgendaOwnerId = userId };
            var result = await useCase.ExecuteAsync(criteria.ToDomain());

            var formattedResult = new PaginatedQuery<PublicContactMAPI, ContactCriteriaMAPI>
            {
                Criteria = criteria,
                Page = result.Page,
                TotalPages = result.TotalPages,
                Results = result.Results.Select(ContactMAPIMapper.FromDomain),
            };

            return Results.Ok(formattedResult);
        });
    }

    private static Task<IResult> SearchContacts(
        ContactCriteriaMAPI criteria,
        ContactQueryUseCase useCase,
        HttpContext ctx,
        RoutesUtils utils
    )
    {
        return utils.HandleResponseAsync(async () =>
        {
            var userId = utils.GetIdFromContext(ctx);
            var result = await useCase.ExecuteAsync(criteria.ToDomain());

            var formattedResult = new PaginatedQuery<PublicContactMAPI, ContactCriteriaMAPI>
            {
                Criteria = criteria,
                Page = result.Page,
                TotalPages = result.TotalPages,
                Results = result.Results.Select(ContactMAPIMapper.FromDomain),
            };

            return Results.Ok(formattedResult);
        });
    }

    private static Task<IResult> DeleteContact(
        ulong agendaOwnerId,
        ulong contactId,
        DeleteContactUseCase useCase,
        HttpContext ctx,
        RoutesUtils utils
    )
    {
        return utils.HandleResponseAsync(async () =>
        {
            var executor = utils.GetExecutorFromContext(ctx);
            var result = await useCase.ExecuteAsync(
                new()
                {
                    Id = new() { AgendaOwnerId = agendaOwnerId, UserId = contactId },
                    Executor = executor,
                }
            );

            if (result.IsErr)
                return result.UnwrapErr().FromDomain();

            return Results.Ok(result.Unwrap().FromDomain());
        });
    }

    private static Task<IResult> UpdateContact(
        ContactUpdateMAPI request,
        UpdateContactUseCase useCase,
        HttpContext ctx,
        RoutesUtils utils
    )
    {
        return utils.HandleResponseAsync(async () =>
        {
            var executor = utils.GetExecutorFromContext(ctx);
            var result = await useCase.ExecuteAsync(request.ToDomain());

            if (result.IsErr)
                return result.UnwrapErr().FromDomain();

            return Results.Ok(result.Unwrap().FromDomain());
        });
    }

    private static Task<IResult> TagsQuery(
        TagCriteriaMAPI criteria,
        TagQueryUseCase useCase,
        HttpContext ctx,
        RoutesUtils utils
    )
    {
        return utils.HandleResponseAsync(async () =>
        {
            var executor = utils.GetExecutorFromContext(ctx);

            var parseTry = criteria.ToDomain();
            if (parseTry.IsErr)
                return parseTry.UnwrapErr().FromDomain();

            var result = await useCase.ExecuteAsync(parseTry.Unwrap());
            return Results.Ok(result);
        });
    }

    private static Task<IResult> AddContactTag(
        ContactTagIdDTO value,
        AddContactTagUseCase useCase,
        HttpContext ctx,
        RoutesUtils utils
    )
    {
        return utils.HandleResponseAsync(async () =>
        {
            var executor = utils.GetExecutorFromContext(ctx);
            var result = await useCase.ExecuteAsync(new() { Id = value, Executor = executor });

            if (result.IsErr)
                return result.UnwrapErr().FromDomain();

            return Results.Created();
        });
    }

    private static Task<IResult> DeleteContactTag(
        ulong agendaOwnerId,
        ulong userId,
        string tag,
        DeleteContactTagUseCase useCase,
        HttpContext ctx,
        RoutesUtils utils
    )
    {
        return utils.HandleResponseAsync(async () =>
        {
            var executor = utils.GetExecutorFromContext(ctx);
            var result = await useCase.ExecuteAsync(
                new()
                {
                    Id = new()
                    {
                        AgendaOwnerId = agendaOwnerId,
                        UserId = userId,
                        Tag = tag,
                    },
                    Executor = executor,
                }
            );

            if (result.IsErr)
                return result.UnwrapErr().FromDomain();

            return Results.Created();
        });
    }
}
