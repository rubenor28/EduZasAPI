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

/// <summary>
/// Caso de uso para la creación de nuevas clases con generación automática de identificador único.
/// </summary>
public class AddClassUseCase(
    ICreatorAsync<ClassDomain, NewClassDTO> creator,
    IBusinessValidationService<NewClassDTO> validator,
    IReaderAsync<ulong, UserDomain> userReader,
    IReaderAsync<string, ClassDomain> reader,
    IRandomStringGeneratorService idGenerator,
    ICreatorAsync<ProfessorClassRelationDTO, ProfessorClassRelationDTO> professorRelationCreator
) : AddUseCase<NewClassDTO, ClassDomain>(creator, validator)
{
    private readonly IEnumerable<UserType> _allowedRoles = [UserType.ADMIN, UserType.PROFESSOR];
    private const int _maxIdGenerationTries = 20;

    protected override async Task<Result<Unit, UseCaseErrorImpl>> ExtraValidationAsync(
        NewClassDTO value
    )
    {
        var usrSearch = await userReader.GetAsync(value.OwnerId);

        if (usrSearch.IsNone)
            return UseCaseError.Input(
                [new() { Field = "ownerId", Message = "No se encontró el usuario" }]
            );

        var usr = usrSearch.Unwrap();
        if (!_allowedRoles.Contains(usr.Role))
        {
            return UseCaseError.Unauthorized();
        }

        return Unit.Value;
    }

    /// <summary>
    /// Genera un identificador único para la clase antes de la persistencia.
    /// </summary>
    /// <param name="value">DTO con los datos de la nueva clase.</param>
    /// <returns>DTO con el identificador único generado.</returns>
    /// <exception cref="InvalidOperationException">
    /// Se lanza cuando no se puede generar un identificador único después del número máximo de intentos.
    /// </exception>
    protected async override Task<NewClassDTO> PostValidationFormatAsync(NewClassDTO value)
    {
        string id;
        for (int i = 0; i < _maxIdGenerationTries; i++)
        {
            id = idGenerator.Generate();
            var repeated = await reader.GetAsync(id);

            if (repeated.IsNone)
            {
                value.Id = id;
                return value;
            }
        }

        throw new InvalidOperationException(
            $"No se pudo generar un identificador único después de {_maxIdGenerationTries} intentos. Es posible que se hayan agotado las combinaciones disponibles."
        );
    }

    /// <summary>
    /// Crea la relación profesor - clase asignando por defecto al
    /// al profesor como dueño
    /// </summary>
    /// <param name="newEntity">DTO con el ID del profesor</param>
    /// <param name="createdEntity">Clase creada con ID generado </param>
    protected async override Task ExtraTaskAsync(NewClassDTO newEntity, ClassDomain createdEntity)
    {
        await professorRelationCreator.AddAsync(
            new ProfessorClassRelationDTO
            {
                Id = new ClassUserRelationIdDTO
                {
                    UserId = newEntity.OwnerId,
                    ClassId = createdEntity.Id,
                },
                IsOwner = true,
            }
        );
    }
}
