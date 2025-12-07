using Application.DAOs;
using Application.DTOs;
using Application.DTOs.Common;
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
using TagReader = IReaderAsync<string, TagDomain>;
using UserReader = IReaderAsync<ulong, UserDomain>;

public sealed class AddContactTagUseCase(
    ContactTagCreator creator,
    TagReader tagReader,
    TagCreator tagCreator,
    UserReader userReader,
    ContactReader contactReader,
    ContactTagReader contactTagReader,
    IBusinessValidationService<NewContactTagDTO>? validator = null
) : AddUseCase<NewContactTagDTO, ContactTagDomain>(creator, validator)
{
    private readonly ContactReader _contactReader = contactReader;
    private readonly ContactTagReader _contactTagReader = contactTagReader;

    private readonly TagReader _tagReader = tagReader;
    private readonly TagCreator _tagCreator = tagCreator;

    private readonly UserReader _userReader = userReader;

    private async Task<Result<Unit, FieldErrorDTO>> SearchUser(ulong userId, string field) =>
        await _userReader.GetAsync(userId) is not null
            ? Unit.Value
            : new FieldErrorDTO { Field = field, Message = "Usuario no encontrado" };

    protected override async Task<Result<Unit, UseCaseError>> ExtraValidationAsync(
        UserActionDTO<NewContactTagDTO> value
    )
    {
        // Buscar errores en campos del DTO
        List<FieldErrorDTO> errors = [];
        (await SearchUser(value.Data.AgendaOwnerId, "agendaOwnerId")).IfErr(errors.Add);
        (await SearchUser(value.Data.UserId, "userId")).IfErr(errors.Add);

        if (errors.Count > 0)
            return UseCaseErrors.Input(errors);

        // Validar permisos
        var authorized = value.Executor.Role switch
        {
            UserType.ADMIN => true,
            UserType.PROFESSOR => value.Data.AgendaOwnerId == value.Executor.Id,
            UserType.STUDENT => false,
            _ => throw new InvalidOperationException(),
        };

        if (!authorized)
            return UseCaseErrors.Unauthorized();

        // Buscar si existe el contacto
        var contact = await _contactReader.GetAsync(
            new() { AgendaOwnerId = value.Data.AgendaOwnerId, UserId = value.Data.UserId }
        );

        if (contact is null)
            return UseCaseErrors.NotFound();

        var existingAssociation = await _contactTagReader.GetAsync(
            new()
            {
                Tag = value.Data.Tag,
                AgendaOwnerId = value.Data.AgendaOwnerId,
                UserId = value.Data.UserId,
            }
        );

        if (existingAssociation is not null)
            return UseCaseErrors.Conflict("El recurso ya existe");

        return Unit.Value;
    }

    protected override async Task PrevTaskAsync(UserActionDTO<NewContactTagDTO> newEntity)
    {
        var tagRecord = await _tagReader.GetAsync(newEntity.Data.Tag);

        if (tagRecord is null)
            await _tagCreator.AddAsync(new() { Text = newEntity.Data.Tag });
    }
}
