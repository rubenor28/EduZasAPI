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
/// Caso de uso para agregar una nueva clase.
/// </summary>
/// <remarks>
///   Este caso de uso maneja la lógica para crear una nueva clase. Hereda de <see cref="AddUseCase{TNewEntity,TEntity}"/>
///   y extiende la funcionalidad para incluir validaciones específicas, la generación de un ID único y la creación
///   de la relación entre la clase y el profesor propietario.
/// </remarks>
public class AddClassUseCase(
    ICreatorAsync<ClassDomain, NewClassDTO> creator,
    IBusinessValidationService<NewClassDTO> validator,
    IReaderAsync<ulong, UserDomain> userReader,
    IReaderAsync<string, ClassDomain> reader,
    IRandomStringGeneratorService idGenerator,
    ICreatorAsync<ClassProfessorDomain, NewClassProfessorDTO> professorRelationCreator
) : AddUseCase<NewClassDTO, ClassDomain>(creator, validator)
{
    /// <summary>
    /// Número máximo de intentos para generar un ID de clase único.
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
    private readonly ICreatorAsync<
        ClassProfessorDomain,
        NewClassProfessorDTO
    > _professorRelationCreator = professorRelationCreator;

    /// <summary>
    /// Realiza validaciones de negocio adicionales antes de crear la clase.
    /// </summary>
    /// <param name="value">DTO con los datos de la nueva clase.</param>
    /// <returns>
    ///   Un <see cref="Result{T, E}"/> que indica si la validación fue exitosa.
    ///   Devuelve <see cref="UseCaseErrors.Unauthorized"/> si el usuario ejecutor no tiene permisos.
    /// </returns>
    /// <exception cref="NotImplementedException">Se lanza si el rol del usuario no está contemplado en la lógica de autorización.</exception>
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

    /// <summary>
    /// Realiza validaciones asíncronas adicionales, como verificar la existencia del usuario propietario.
    /// </summary>
    /// <param name="value">DTO con los datos de la nueva clase.</param>
    /// <returns>
    ///   Un <see cref="Result{T, E}"/> que indica si la validación fue exitosa.
    ///   Devuelve un error de <see cref="UseCaseErrors.Input"/> si el usuario propietario no se encuentra.
    /// </returns>
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

    /// <summary>
    /// Formatea el DTO después de la validación, principalmente para generar y asignar un ID único a la clase.
    /// </summary>
    /// <param name="value">DTO con los datos de la nueva clase.</param>
    /// <returns>El DTO modificado con el nuevo ID.</returns>
    /// <exception cref="InvalidOperationException">
    ///     Se lanza si no se puede generar un ID único después del número máximo de intentos.
    /// </exception>
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

    /// <summary>
    /// Ejecuta tareas adicionales después de crear la entidad principal. En este caso, crea la relación
    /// que designa al usuario propietario como profesor de la clase.
    /// </summary>
    /// <param name="newEntity">El DTO original con los datos de la nueva clase.</param>
    /// <param name="createdEntity">La entidad de dominio de la clase recién creada.</param>
    /// <returns>Una tarea que representa la operación asíncrona.</returns>
    protected override async Task ExtraTaskAsync(NewClassDTO newEntity, ClassDomain createdEntity)
    {
        await _professorRelationCreator.AddAsync(
            new()
            {
                ClassId = createdEntity.Id,
                UserId = newEntity.OwnerId,
                IsOwner = true,
                Executor = new() { Id = 1, Role = UserType.ADMIN },
            }
        );
    }
}
