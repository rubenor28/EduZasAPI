using Application.DAOs;
using Application.DTOs;
using Application.DTOs.Classes;
using Application.DTOs.ClassProfessors;
using Application.DTOs.Common;
using Application.Services;
using Application.UseCases.Common;
using Domain.Entities;
using Domain.Enums;
using Domain.Extensions;
using Domain.ValueObjects;

namespace Application.UseCases.Classes;

/// <summary>
/// Caso de uso para crear nuevas clases.
/// </summary>
/// <remarks>
/// Genera un ID único y asigna al creador como profesor propietario.
/// </remarks>
public class AddClassUseCase(
    ICreatorAsync<ClassDomain, NewClassDTO> creator,
    IBusinessValidationService<NewClassDTO> validator,
    IReaderAsync<ulong, UserDomain> userReader,
    IReaderAsync<string, ClassDomain> reader,
    IRandomStringGeneratorService idGenerator,
    IBulkCreatorAsync<ClassProfessorDomain, NewClassProfessorDTO> professorRelationCreator
) : AddUseCase<NewClassDTO, ClassDomain>(creator, validator)
{
    /// <summary>
    /// Intentos máximos para generar un ID único.
    /// </summary>
    private const int _maxIdGenerationTries = 20;

    /// <summary>
    /// Servicio para leer datos de clases.
    /// </summary>
    private readonly IReaderAsync<string, ClassDomain> _reader = reader;

    /// <summary>
    /// Servicio para generar cadenas de texto aleatorias para el ID de la clase.
    /// </summary>
    private readonly IRandomStringGeneratorService _idGenerator = idGenerator;

    /// <summary>
    /// Servicio para crear la relación entre un profesor y una clase.
    /// </summary>
    private readonly IBulkCreatorAsync<
        ClassProfessorDomain,
        NewClassProfessorDTO
    > _professorRelationCreator = professorRelationCreator;

    /// <inheritdoc/>
    protected override Result<Unit, UseCaseError> ExtraValidation(UserActionDTO<NewClassDTO> value)
    {
        var authorized = value.Executor.Role switch
        {
            UserType.ADMIN => true,
            UserType.PROFESSOR => value.Data.OwnerId == value.Executor.Id,
            UserType.STUDENT => false,
            _ => throw new NotImplementedException(),
        };

        if (!authorized)
            return UseCaseErrors.Unauthorized();

        return Unit.Value;
    }

    /// <inheritdoc/>
    protected override async Task<Result<Unit, UseCaseError>> ExtraValidationAsync(
        UserActionDTO<NewClassDTO> value
    )
    {
        List<FieldErrorDTO> errors = [];

        (await userReader.GetAsync(value.Data.OwnerId)).IfNull(() =>
            errors.Add(new() { Field = "ownerId", Message = "No se encontró el usuario" })
        );

        var validationTasks = value.Data.Professors.Select(async professor =>
        {
            var userResult = await userReader.GetAsync(professor.UserId);
            return userResult.Match(
                user => user.Role == UserType.STUDENT ? professor.UserId : (ulong?)null,
                () => professor.UserId
            );
        });

        var invalidIdResults = await Task.WhenAll(validationTasks);
        var invalidProfessorIds = invalidIdResults
            .Where(id => id is not null)
            .Select(id => id!.Value)
            .ToList();

        if (invalidProfessorIds.Count > 0)
        {
            var idList = string.Join(", ", invalidProfessorIds);
            errors.Add(
                new()
                {
                    Field = "professors",
                    Message =
                        $"Los siguientes usuarios no son válidos o son estudiantes: [{idList}]",
                }
            );
        }

        if (errors.Count > 0)
            return UseCaseErrors.Input(errors);

        return Unit.Value;
    }

    /// <inheritdoc/>
    protected override async Task<UserActionDTO<NewClassDTO>> PostValidationFormatAsync(UserActionDTO<NewClassDTO> value)
    {
        for (int i = 0; i < _maxIdGenerationTries; i++)
        {
            var newId = _idGenerator.Generate();
            var repeated = await _reader.GetAsync(newId);

            if (repeated is null)
            {
                return value with { Data = value.Data with { Id = newId } };
            }
        }

        throw new InvalidOperationException(
            $"Tras {_maxIdGenerationTries} intentos no se pudo generar un ID unico a una clase"
        );
    }

    /// <inheritdoc/>
    protected override async Task ExtraTaskAsync(
        UserActionDTO<NewClassDTO> newEntity,
        ClassDomain createdEntity
    )
    {
        newEntity.Data.Professors.Add(new() { UserId = newEntity.Data.OwnerId, IsOwner = true });

        var professors = newEntity.Data.Professors.Select(p => new NewClassProfessorDTO()
        {
            ClassId = createdEntity.Id,
            UserId = p.UserId,
            IsOwner = p.IsOwner,
        });

        await _professorRelationCreator.AddRangeAsync(professors);
    }
}
