using Application.DAOs;
using Application.DTOs;
using Application.DTOs.Common;
using Application.DTOs.Contacts;
using Application.DTOs.ContactTags;
using Application.DTOs.Tags;
using Application.Services;
using Application.UseCases.Common;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;

namespace Application.UseCases.ContactTags;

using ContactReader = IReaderAsync<ContactIdDTO, ContactDomain>;
using ContactTagCreator = ICreatorAsync<ContactTagDomain, NewContactTagDTO>;
using ContactTagReader = IReaderAsync<ContactTagIdDTO, ContactTagDomain>;
using TagCreator = ICreatorAsync<TagDomain, NewTagDTO>;
using TagQuerier = IQuerierAsync<TagDomain, TagCriteriaDTO>;
using UserReader = IReaderAsync<ulong, UserDomain>;

public sealed class AddContactTagUseCase(
    ContactTagCreator contactTagCreator,
    TagQuerier tagQuerier,
    TagCreator tagCreator,
    UserReader userReader,
    ContactReader contactReader,
    ContactTagReader contactTagReader,
    IBusinessValidationService<NewContactTagRequestDTO>? validator = null
) : IUseCaseAsync<NewContactTagRequestDTO, ContactTagDomain>
{
    private readonly ContactReader _contactReader = contactReader;
    private readonly ContactTagCreator _contactTagCreator = contactTagCreator;
    private readonly ContactTagReader _contactTagReader = contactTagReader;
    private readonly TagCreator _tagCreator = tagCreator;
    private readonly TagQuerier _tagQuerier = tagQuerier;
    private readonly UserReader _userReader = userReader;
    private readonly IBusinessValidationService<NewContactTagRequestDTO>? _validator = validator;

    public async Task<Result<ContactTagDomain, UseCaseError>> ExecuteAsync(
        UserActionDTO<NewContactTagRequestDTO> value
    )
    {
        if (_validator is not null)
        {
            var validationResult = _validator.IsValid(value.Data);
            if (validationResult.IsErr)
                return UseCaseErrors.Input(validationResult.UnwrapErr());
        }

        var extraValidationResult = await ExtraValidationAsync(value);
        if (extraValidationResult.IsErr)
            return extraValidationResult.UnwrapErr();

        var tags = await _tagQuerier.GetByAsync(
            new() { Text = new StringQueryDTO{ Text = value.Data.TagText.Trim().ToUpperInvariant(), SearchType = StringSearchType.EQ }, PageSize = 1 }
        );

        var tag = tags.Results.FirstOrDefault();
        tag ??= await _tagCreator.AddAsync(new() { Text = value.Data.TagText.Trim().ToUpperInvariant() });

        var association = await _contactTagReader.GetAsync(
            new()
            {
                TagId = tag.Id,
                AgendaOwnerId = value.Data.AgendaOwnerId,
                UserId = value.Data.UserId
            }
        );

        if (association is not null)
            return UseCaseErrors.Conflict("La etiqueta ya está asignada a este contacto.");

        var result = await _contactTagCreator.AddAsync(
            new()
            {
                TagId = tag.Id,
                AgendaOwnerId = value.Data.AgendaOwnerId,
                UserId = value.Data.UserId
            }
        );

        return result;
    }

    private async Task<Result<Unit, UseCaseError>> ExtraValidationAsync(
        UserActionDTO<NewContactTagRequestDTO> value
    )
    {
        List<FieldErrorDTO> errors = [];

        async Task<Result<Unit, FieldErrorDTO>> searchUser(ulong userId, string field) =>
            await _userReader.GetAsync(userId) is not null
                ? Unit.Value
                : new FieldErrorDTO { Field = field, Message = "Usuario no encontrado" };

        (await searchUser(value.Data.AgendaOwnerId, "agendaOwnerId")).IfErr(errors.Add);
        (await searchUser(value.Data.UserId, "userId")).IfErr(errors.Add);

        if (errors.Count > 0)
            return UseCaseErrors.Input(errors);

        var authorized = value.Executor.Role switch
        {
            UserType.ADMIN => true,
            UserType.PROFESSOR => value.Data.AgendaOwnerId == value.Executor.Id,
            UserType.STUDENT => false,
            _ => throw new InvalidOperationException(),
        };

        if (!authorized)
            return UseCaseErrors.Unauthorized();

        var contact = await _contactReader.GetAsync(
            new() { AgendaOwnerId = value.Data.AgendaOwnerId, UserId = value.Data.UserId }
        );

        if (contact is null)
            return UseCaseErrors.NotFound();

        return Unit.Value;
    }
}
