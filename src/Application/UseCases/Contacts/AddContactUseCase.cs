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
using Domain.Extensions;
using Domain.ValueObjects;

namespace Application.UseCases.Contacts;

using ContactCreator = ICreatorAsync<ContactDomain, NewContactDTO>;
using ContactTagCreator = ICreatorAsync<ContactTagDomain, NewContactTagDTO>;
using TagCreator = ICreatorAsync<TagDomain, NewTagDTO>;
using TagReader = IReaderAsync<string, TagDomain>;
using UserReader = IReaderAsync<ulong, UserDomain>;

public sealed class AddContactUseCase(
    ContactCreator creator,
    IReaderAsync<ContactIdDTO, ContactDomain> reader,
    UserReader userReader,
    TagReader tagReader,
    TagCreator tagCreator,
    ContactTagCreator contactTagCreator,
    IBusinessValidationService<NewContactDTO>? validator = null
) : AddUseCase<NewContactDTO, ContactDomain>(creator, validator)
{
    private readonly IReaderAsync<ContactIdDTO, ContactDomain> _reader = reader;

    private readonly UserReader _userReader = userReader;

    private readonly TagReader _tagReader = tagReader;
    private readonly TagCreator _tagCreator = tagCreator;

    private readonly ContactTagCreator _contactTagCreator = contactTagCreator;

    protected override async Task<Result<Unit, UseCaseError>> ExtraValidationAsync(
        UserActionDTO<NewContactDTO> request
    )
    {
        List<FieldErrorDTO> errors = [];

        (await SearchUser(request.Data.AgendaOwnerId, "agendaOwnerId")).IfErr(errors.Add);
        (await SearchUser(request.Data.UserId, "userId")).IfErr(errors.Add);
        if (request.Data.UserId == request.Executor.Id)
            errors.Add(new() { Field = "userId", Message = "No puedes agregarte a ti mismo" });

        if (errors.Count != 0)
            return UseCaseErrors.Input(errors);

        var authorized = request.Executor.Role switch
        {
            UserType.ADMIN => true,
            _ => request.Executor.Id == request.Data.AgendaOwnerId,
        };

        if (!authorized)
            return UseCaseErrors.Unauthorized();

        var contact = await _reader.GetAsync(
            new() { UserId = request.Data.UserId, AgendaOwnerId = request.Data.AgendaOwnerId }
        );

        if (contact is not null)
            return UseCaseErrors.AlreadyExists();

        return Unit.Value;
    }

    protected override async Task ExtraTaskAsync(
        UserActionDTO<NewContactDTO> newEntity,
        ContactDomain createdEntity
    )
    {
        await newEntity.Data.Tags.IfSome(
            async (tags) =>
            {
                var normalizedTags = tags.Distinct().ToList();
                foreach (var t in normalizedTags)
                {
                    await _tagReader
                        .GetAsync(t)
                        .Match(
                            tag => Task.CompletedTask,
                            async () => await _tagCreator.AddAsync(new() { Text = t })
                        );

                    await _contactTagCreator.AddAsync(
                        new()
                        {
                            AgendaOwnerId = newEntity.Data.AgendaOwnerId,
                            UserId = newEntity.Data.UserId,
                            Tag = t,
                        }
                    );
                }
            }
        );
    }

    private Task<Result<Unit, FieldErrorDTO>> SearchUser(ulong id, string field) =>
        _userReader
            .GetAsync(id)
            .Match<UserDomain, Result<Unit, FieldErrorDTO>>(
                async user => Unit.Value,
                async () => new FieldErrorDTO { Field = field, Message = "Usuario no encontrado" }
            );
}
