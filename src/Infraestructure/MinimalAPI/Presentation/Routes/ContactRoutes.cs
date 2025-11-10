using Application.DTOs.Common;
using Application.DTOs.Contacts;
using Application.DTOs.ContactTags;
using Application.DTOs.Tags;
using Application.UseCases.Contacts;
using Application.UseCases.ContactTags;
using Application.UseCases.Tags;
using Domain.Entities;
using Domain.ValueObjects;
using InterfaceAdapters.Mappers.Common;
using MinimalAPI.Application.DTOs.Contacts;
using MinimalAPI.Application.DTOs.Tags;
using MinimalAPI.Presentation.Filters;

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
        IMapper<NewContactMAPI, Executor, NewContactDTO> reqMapper,
        IMapper<ContactDomain, PublicContactMAPI> resMapper,
        HttpContext ctx,
        RoutesUtils utils
    )
    {
        return utils.HandleUseCaseAsync(
            useCase,
            mapRequest: () => reqMapper.Map(request, utils.GetExecutorFromContext(ctx)),
            mapResponse: (c) => Results.Created($"/contacts/{c.Id}", resMapper.Map(c))
        );
    }

    private static Task<IResult> SearchMyContacts(
        ContactCriteriaMAPI criteria,
        ContactQueryUseCase useCase,
        IMapper<ContactCriteriaMAPI, ContactCriteriaDTO> reqMapper,
        IMapper<
            PaginatedQuery<ContactDomain, ContactCriteriaDTO>,
            PaginatedQuery<PublicContactMAPI, ContactCriteriaMAPI>
        > resMapper,
        HttpContext ctx,
        RoutesUtils utils
    )
    {
        return utils.HandleUseCaseAsync(
            useCase,
            mapRequest: () =>
                reqMapper.Map(criteria with { AgendaOwnerId = utils.GetIdFromContext(ctx) }),
            mapResponse: (s) => Results.Ok(resMapper.Map(s))
        );
    }

    private static Task<IResult> SearchContacts(
        ContactCriteriaMAPI criteria,
        ContactQueryUseCase useCase,
        IMapper<ContactCriteriaMAPI, ContactCriteriaDTO> reqMapper,
        IMapper<
            PaginatedQuery<ContactDomain, ContactCriteriaDTO>,
            PaginatedQuery<PublicContactMAPI, ContactCriteriaMAPI>
        > resMapper,
        HttpContext ctx,
        RoutesUtils utils
    )
    {
        return utils.HandleUseCaseAsync(
            useCase,
            mapRequest: () => reqMapper.Map(criteria),
            mapResponse: (search) => Results.Ok(resMapper.Map(search))
        );
    }

    private static Task<IResult> DeleteContact(
        ulong agendaOwnerId,
        ulong contactId,
        DeleteContactUseCase useCase,
        IMapper<ulong, ulong, Executor, DeleteContactDTO> reqMapper,
        IMapper<ContactDomain, PublicContactMAPI> resMapper,
        HttpContext ctx,
        RoutesUtils utils
    )
    {
        return utils.HandleUseCaseAsync(
            useCase,
            mapRequest: () =>
                reqMapper.Map(agendaOwnerId, contactId, utils.GetExecutorFromContext(ctx)),
            mapResponse: (contact) => Results.Ok(resMapper.Map(contact))
        );
    }

    private static Task<IResult> UpdateContact(
        ContactUpdateMAPI request,
        UpdateContactUseCase useCase,
        IMapper<ContactUpdateMAPI, ContactUpdateDTO> reqMapper,
        IMapper<ContactDomain, PublicContactMAPI> resMapper,
        HttpContext ctx,
        RoutesUtils utils
    )
    {
        return utils.HandleUseCaseAsync(
            useCase,
            mapRequest: () => reqMapper.Map(request),
            mapResponse: (contact) => Results.Ok(resMapper.Map(contact))
        );
    }

    private static Task<IResult> TagsQuery(
        TagCriteriaMAPI criteria,
        TagQueryUseCase useCase,
        IMapper<TagCriteriaMAPI, Result<TagCriteriaDTO, IEnumerable<FieldErrorDTO>>> reqMapper,
        IMapper<
            PaginatedQuery<TagDomain, TagCriteriaDTO>,
            PaginatedQuery<string, TagCriteriaMAPI>
        > resMapper,
        HttpContext ctx,
        RoutesUtils utils
    )
    {
        return utils.HandleUseCaseAsync(
            useCase,
            mapRequest: () => reqMapper.Map(criteria),
            mapResponse: (search) => Results.Ok(resMapper.Map(search))
        );
    }

    private static Task<IResult> AddContactTag(
        ContactTagIdDTO value,
        AddContactTagUseCase useCase,
        IMapper<ContactTagIdDTO, Executor, NewContactTagDTO> reqMapper,
        HttpContext ctx,
        RoutesUtils utils
    )
    {
        return utils.HandleUseCaseAsync(
            useCase,
            mapRequest: () => reqMapper.Map(value, utils.GetExecutorFromContext(ctx)),
            mapResponse: (_) => Results.NoContent()
        );
    }

    private static Task<IResult> DeleteContactTag(
        ulong agendaOwnerId,
        ulong userId,
        string tag,
        DeleteContactTagUseCase useCase,
        IMapper<ulong, ulong, string, Executor, DeleteContactTagDTO> reqMapper,
        HttpContext ctx,
        RoutesUtils utils
    )
    {
        return utils.HandleUseCaseAsync(
            useCase,
            mapRequest: () =>
                reqMapper.Map(agendaOwnerId, userId, tag, utils.GetExecutorFromContext(ctx)),
            mapResponse: _ => Results.NoContent()
        );
    }
}
