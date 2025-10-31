using Application.DTOs.Common;
using Application.DTOs.Contacts;
using Application.UseCases.Common;
using Application.UseCases.Contacts;
using Domain.Entities;
using Domain.ValueObjects;
using MinimalAPI.Application.DTOs.Contacts;
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

        app.MapDelete("/", DeleteContact)
            .WithName("Eliminar contacto")
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
                    ContactId = request.ContactId,
                    ContactTags = request.ContactTags,
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
        QueryUseCase<ContactCriteriaDTO, ContactDomain> useCase,
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
        QueryUseCase<ContactCriteriaDTO, ContactDomain> useCase,
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
        ulong contactId,
        DeleteContactUseCase useCase,
        HttpContext ctx,
        RoutesUtils utils
    )
    {
        return utils.HandleResponseAsync(async () =>
        {
            var executor = utils.GetExecutorFromContext(ctx);
            var result = await useCase.ExecuteAsync(new() { Id = contactId, Executor = executor });

            if (result.IsErr)
                return result.UnwrapErr().FromDomain();

            return Results.Ok(result.Unwrap().FromDomain());
        });
    }
}
