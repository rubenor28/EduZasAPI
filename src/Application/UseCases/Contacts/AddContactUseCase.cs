using Application.DAOs;
using Application.DTOs.Common;
using Application.DTOs.Contacts;
using Application.DTOs.ContactTags;
using Application.DTOs.Tags;
using Application.Services;
using Application.UseCases.Common;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;

namespace Application.UseCases.Contacts;

using ContactCreator = ICreatorAsync<ContactDomain, NewContactDTO>;
using ContactQuerier = IQuerierAsync<ContactDomain, ContactCriteriaDTO>;
using ContactTagCreator = ICreatorAsync<ContactTagDomain, NewContactTagDTO>;
using TagCreator = ICreatorAsync<TagDomain, NewTagDTO>;
using TagReader = IReaderAsync<string, TagDomain>;
using UserReader = IReaderAsync<ulong, UserDomain>;

public sealed class AddContactUseCase(
    ContactCreator creator,
    ContactQuerier querier,
    UserReader userReader,
    TagReader tagReader,
    TagCreator tagCreator,
    ContactTagCreator contactTagCreator,
    IBusinessValidationService<NewContactDTO>? validator = null
) : AddUseCase<NewContactDTO, ContactDomain>(creator, validator)
{
    private readonly ContactQuerier _querier = querier;

    private readonly UserReader _userReader = userReader;

    private readonly TagReader _tagReader = tagReader;
    private readonly TagCreator _tagCreator = tagCreator;

    private readonly ContactTagCreator _contactTagCreator = contactTagCreator;

    protected override async Task<Result<Unit, UseCaseErrorImpl>> ExtraValidationAsync(
        NewContactDTO request
    )
    {
        List<FieldErrorDTO> errors = [];

        (await SearchUser(request.AgendaOwnerId, "agendaOwnerId")).IfErr(errors.Add);
        (await SearchUser(request.UserId, "userId")).IfErr(errors.Add);

        if (errors.Count != 0)
            return UseCaseError.Input(errors);

        var authorized = request.Executor.Role switch
        {
            UserType.ADMIN => true,
            _ => request.Executor.Id == request.AgendaOwnerId,
        };

        if (!authorized)
            return UseCaseError.Unauthorized();

        var repeatedSearch = await _querier.GetByAsync(
            new() { AgendaOwnerId = request.AgendaOwnerId, UserId = request.UserId }
        );

        if (repeatedSearch.Results.Any())
            return UseCaseError.AlreadyExists();

        return Unit.Value;
    }

    protected override async Task ExtraTaskAsync(
        NewContactDTO newEntity,
        ContactDomain createdEntity
    )
    {
        await newEntity.Tags.IfSomeAsync(
            async (tags) =>
            {
                var normalizedTags = tags.Distinct().ToList();
                foreach (var tag in normalizedTags)
                {
                    var tagSearch = await _tagReader.GetAsync(tag);

                    var tagInstance = tagSearch.IsSome
                        ? tagSearch.Unwrap()
                        : await _tagCreator.AddAsync(new() { Text = tag });

                    await _contactTagCreator.AddAsync(
                        new()
                        {
                            Tag = tagInstance.Text,
                            ContactId = createdEntity.Id,
                            Executor = newEntity.Executor,
                        }
                    );
                }
            }
        );
    }

    private async Task<Result<Unit, FieldErrorDTO>> SearchUser(ulong id, string field)
    {
        var userSearch = await _userReader.GetAsync(id);
        if (userSearch.IsNone)
            return new FieldErrorDTO { Field = field, Message = "Usuario no encontrado" };

        return Unit.Value;
    }
}
