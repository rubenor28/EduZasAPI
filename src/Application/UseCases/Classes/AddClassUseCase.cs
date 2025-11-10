using Application.DAOs;
using Application.DTOs.Classes;
using Application.DTOs.ClassProfessors;
using Application.DTOs.Common;
using Application.Services;
using Application.UseCases.Common;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;

namespace Application.UseCases.Classes;

public class AddClassUseCase(
    ICreatorAsync<ClassDomain, NewClassDTO> creator,
    IBusinessValidationService<NewClassDTO> validator,
    IReaderAsync<ulong, UserDomain> userReader,
    IReaderAsync<string, ClassDomain> reader,
    IRandomStringGeneratorService idGenerator,
    ICreatorAsync<ProfessorClassRelationDTO, ProfessorClassRelationDTO> professorRelationCreator
) : AddUseCase<NewClassDTO, ClassDomain>(creator, validator)
{
    private const int _maxIdGenerationTries = 20;
    private readonly IReaderAsync<string, ClassDomain> _reader = reader;
    private readonly IRandomStringGeneratorService _idGenerator = idGenerator;
    private readonly ICreatorAsync<
        ProfessorClassRelationDTO,
        ProfessorClassRelationDTO
    > _professorRelationCreator = professorRelationCreator;

    protected override Result<Unit, UseCaseError> ExtraValidation(NewClassDTO value)
    {
        var authorized = value.Executor.Role switch
        {
            UserType.ADMIN => true,
            UserType.PROFESSOR => value.OwnerId == value.Executor.Id,
            UserType.STUDENT => false,
            _ => throw new NotImplementedException(),
        };

        if (!authorized)
            return UseCaseErrors.Unauthorized();

        return Unit.Value;
    }

    protected override async Task<Result<Unit, UseCaseError>> ExtraValidationAsync(
        NewClassDTO value
    )
    {
        var usrSearch = await userReader.GetAsync(value.OwnerId);

        if (usrSearch.IsNone)
            return UseCaseErrors.Input(
                [new() { Field = "ownerId", Message = "No se encontró el usuario" }]
            );

        return Unit.Value;
    }

    protected override async Task<NewClassDTO> PostValidationFormatAsync(NewClassDTO value)
    {
        for (int i = 0; i < _maxIdGenerationTries; i++)
        {
            var newId = _idGenerator.Generate();
            var repeated = await _reader.GetAsync(newId);

            if (repeated.IsNone)
            {
                value.Id = newId;
                return value;
            }
        }

        throw new InvalidOperationException(
            $"Tras {_maxIdGenerationTries} intentos no se pudo generar un ID unico a una clase"
        );
    }

    protected override async Task ExtraTaskAsync(NewClassDTO newEntity, ClassDomain createdEntity)
    {
        await _professorRelationCreator.AddAsync(
            new()
            {
                Id = new() { ClassId = createdEntity.Id, UserId = newEntity.OwnerId },
                IsOwner = true,
            }
        );
    }
}
