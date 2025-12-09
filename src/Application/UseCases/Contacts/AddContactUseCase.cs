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

/// <summary>
/// Caso de uso para añadir un contacto a la agenda.
/// </summary>
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

    /// <inheritdoc/>
    protected override async Task<Result<Unit, UseCaseError>> ExtraValidationAsync(
        UserActionDTO<NewContactDTO> request
    )
    {
        List<FieldErrorDTO> errors = [];

        if (request.Data.UserId == request.Executor.Id)
            errors.Add(new() { Field = "userId", Message = "No puedes agregarte a ti mismo" });

        (await SearchUser(request.Data.AgendaOwnerId, "agendaOwnerId")).IfErr(errors.Add);
        (await SearchUser(request.Data.UserId, "userId")).IfErr(errors.Add);

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
            return UseCaseErrors.Conflict("El recurso ya existe");

        return Unit.Value;
    }

    /// <inheritdoc/>
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
                    var tag = await _tagReader.GetAsync(t);
                    if (tag is null)
                    {
                        await _tagCreator.AddAsync(new() { Text = t });
                    }

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

    private async Task<Result<Unit, FieldErrorDTO>> SearchUser(ulong id, string field)
    {
        var user = await _userReader.GetAsync(id);
        if (user is null)
            return new FieldErrorDTO { Field = field, Message = "Usuario no encontrado" };

        return Unit.Value;
    }
}
