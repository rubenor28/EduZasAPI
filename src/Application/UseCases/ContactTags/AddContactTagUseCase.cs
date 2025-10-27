using Application.DAOs;
using Application.DTOs.Common;
using Application.DTOs.ContactTag;
using Application.Services;
using Application.UseCases.Common;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;

namespace Application.UseCases.ContactTags;

public sealed class AddContactClassUseCase(
    ICreatorAsync<ContactTagDomain, NewContactTagDTO> creator,
    IReaderAsync<ulong, ContactDomain> contactReader,
    IReaderAsync<ulong, TagDomain> tagReader,
    IReaderAsync<ContactTagIdDTO, ContactTagDomain> contactTagReader,
    IBusinessValidationService<NewContactTagDTO>? validator = null
) : AddUseCase<NewContactTagDTO, ContactTagDomain>(creator, validator)
{
    private readonly IReaderAsync<ulong, ContactDomain> _contactReader = contactReader;
    private readonly IReaderAsync<ulong, TagDomain> _tagReader = tagReader;
    private readonly IReaderAsync<ContactTagIdDTO, ContactTagDomain> _contactTagReader =
        contactTagReader;

    protected override async Task<Result<Unit, UseCaseErrorImpl>> ExtraValidationAsync(
        NewContactTagDTO request
    )
    {
        var contactSearch = await _contactReader.GetAsync(request.AgendaContactId);
        var tagSearch = await _tagReader.GetAsync(request.TagId);

        List<FieldErrorDTO> errors = [];

        if (contactSearch.IsNone)
            errors.Add(new() { Field = "agendaContactId", Message = "No se encontró el contacto" });

        if (tagSearch.IsNone)
            errors.Add(new() { Field = "tagId", Message = "No se encontró la etiqueta" });

        if (errors.Count != 0)
            return UseCaseError.Input(errors);

        var contactTagSearch = await _contactTagReader.GetAsync(
            new() { AgendaContactId = request.AgendaContactId, TagId = request.TagId }
        );

        if (contactTagSearch.IsSome)
            return UseCaseError.AlreadyExists();

        var authorizedToModify = request.Executor.Role switch
        {
            UserType.ADMIN => true,
            UserType.STUDENT => false,
            UserType.PROFESSOR => request.Executor.Id == contactSearch.Unwrap().AgendaOwnerId,
            _ => throw new NotImplementedException(),
        };

        if(!authorizedToModify)
          return UseCaseError.Unauthorized();

        return Unit.Value;
    }
}
