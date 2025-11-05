using Application.DAOs;
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
using ContactTagCreator = ICreatorAsync<ContactTagDomain, ContactTagDTO>;
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
    IBusinessValidationService<ContactTagDTO>? validator = null
) : AddUseCase<ContactTagDTO, ContactTagDomain>(creator, validator)
{
    private readonly ContactReader _contactReader = contactReader;
    private readonly ContactTagReader _contactTagReader = contactTagReader;

    private readonly TagReader _tagReader = tagReader;
    private readonly TagCreator _tagCreator = tagCreator;

    private readonly UserReader _userReader = userReader;

    private async Task CreateTagIfNotExists(string tag)
    {
        var tagRecord = await _tagReader.GetAsync(tag);

        if (tagRecord.IsNone)
            await _tagCreator.AddAsync(new() { Text = tag });
    }

    private async Task<Result<Unit, FieldErrorDTO>> SearchUser(ulong userId, string field)
    {
        return (await _userReader.GetAsync(userId)).IsSome
            ? Unit.Value
            : new FieldErrorDTO { Field = field, Message = "Usuario no encontrado" };
    }

    protected override async Task<Result<Unit, UseCaseErrorImpl>> ExtraValidationAsync(
        ContactTagDTO value
    )
    {
        // Buscar errores en campos del DTO
        List<FieldErrorDTO> errors = [];
        (await SearchUser(value.Id.AgendaOwnerId, "agendaOwnerId")).IfErr(errors.Add);
        (await SearchUser(value.Id.UserId, "userId")).IfErr(errors.Add);

        if (errors.Count > 0)
            return UseCaseError.Input(errors);

        // Buscar si existe el contacto
        var contact = await _contactReader.GetAsync(
            new() { AgendaOwnerId = value.Id.AgendaOwnerId, UserId = value.Id.UserId }
        );

        if (contact.IsNone)
            return UseCaseError.NotFound();

        var existingAssociation = await _contactTagReader.GetAsync(value.Id);
        if (existingAssociation.IsSome)
            return UseCaseError.AlreadyExists();

        // Validar permisos
        var authorized = value.Executor.Role switch
        {
            UserType.ADMIN => true,
            UserType.PROFESSOR => value.Id.AgendaOwnerId == value.Executor.Id,
            UserType.STUDENT => false,
            _ => throw new InvalidOperationException(),
        };

        if (!authorized)
            return UseCaseError.Unauthorized();

        return Unit.Value;
    }

    protected override async Task PrevTaskAsync(ContactTagDTO newEntity) =>
        await CreateTagIfNotExists(newEntity.Id.Tag);
}
