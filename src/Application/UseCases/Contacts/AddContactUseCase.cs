using Application.DAOs;
using Application.DTOs.Common;
using Application.DTOs.Contacts;
using Application.DTOs.Tags;
using Application.Services;
using Application.UseCases.Common;
using Application.UseCases.ContactTags;
using Application.UseCases.Tags;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;

namespace Application.UseCases.Contacts;

public sealed class AddContactUseCase(
    ICreatorAsync<ContactDomain, NewContactDTO> creator,
    IReaderAsync<ulong, UserDomain> userReader,
    IQuerierAsync<ContactDomain, ContactCriteriaDTO> contactQuerier,
    IQuerierAsync<TagDomain, TagCriteriaDTO> tagQuerier,
    AddContactTagUseCase addContactTagUseCase,
    AddTagUseCase addTagUseCase,
    IBusinessValidationService<NewContactDTO>? validator = null
) : AddUseCase<NewContactDTO, ContactDomain>(creator, validator)
{
    // Manejo de relacion Etiqueta - Profesor
    private readonly IQuerierAsync<TagDomain, TagCriteriaDTO> _tagQuerier = tagQuerier;
    private readonly IReaderAsync<ulong, UserDomain> _userReader = userReader;
    private readonly IQuerierAsync<ContactDomain, ContactCriteriaDTO> _contactQuerier =
        contactQuerier;

    // Funcionalidad extra para asignacion de etiquetas desde creacion
    private readonly AddTagUseCase _addTagUseCase = addTagUseCase;
    private readonly AddContactTagUseCase _addContactTagUseCase = addContactTagUseCase;

    protected override async Task<Result<Unit, UseCaseErrorImpl>> ExtraValidationAsync(
        NewContactDTO request
    )
    {
        List<FieldErrorDTO> errors = [];

        var agendaOwnerSearch = await _userReader.GetAsync(request.AgendaOwnerId);
        if (agendaOwnerSearch.IsNone)
            errors.Add(new() { Field = "agendaOwnerId", Message = "Usuario no encontrado" });

        var contactIdSearch = await _userReader.GetAsync(request.ContactId);
        if (contactIdSearch.IsNone)
            errors.Add(new() { Field = "contactId", Message = "Usuario no encontrado" });

        if (errors.Count != 0)
            return UseCaseError.Input(errors);

        var userExistenceSearch = await _contactQuerier.GetByAsync(
            new() { AgendaOwnerId = request.AgendaOwnerId, ContactId = request.ContactId }
        );

        if (userExistenceSearch.Results.Any())
            return UseCaseError.AlreadyExists();

        var authorized = request.Executor.Role switch
        {
            UserType.ADMIN => true,
            _ => request.Executor.Id == request.AgendaOwnerId,
        };

        if (!authorized)
            UseCaseError.Unauthorized();

        return Unit.Value;
    }

    protected override async Task ExtraTaskAsync(
        NewContactDTO newEntity,
        ContactDomain createdEntity
    )
    {
        foreach (var tagToLink in newEntity.ContactTags)
        {
            var tagSearch = await _tagQuerier.GetByAsync(
                new()
                {
                    Text = new StringQueryDTO
                    {
                        Text = tagToLink.Text,
                        SearchType = StringSearchType.EQ,
                    },
                }
            );

            var tagRecord = tagSearch.Results.Any() switch
            {
                true => tagSearch.Results.First(),
                false => await CreateTag(tagToLink),
            };

            var validation = await _addContactTagUseCase.ExecuteAsync(
                new()
                {
                    TagId = tagRecord.Id,
                    AgendaContactId = newEntity.AgendaOwnerId,
                    Executor = newEntity.Executor,
                }
            );

            if(validation.IsErr) throw new Exception("No se pudo vincular la etiqueta");
        }
    }

    private async Task<TagDomain> CreateTag(NewTagDTO data)
    {
        var result = await _addTagUseCase.ExecuteAsync(data);

        if (result.IsErr)
            throw new Exception("No se pudieron crear las etiquetas");

        return result.Unwrap();
    }
}
