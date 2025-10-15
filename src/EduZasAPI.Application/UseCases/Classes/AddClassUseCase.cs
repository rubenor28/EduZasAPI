using EduZasAPI.Domain.Common;
using EduZasAPI.Domain.Users;
using EduZasAPI.Domain.Classes;
using EduZasAPI.Application.Common;

namespace EduZasAPI.Application.Classes;

/// <summary>
/// Caso de uso para la creación de nuevas clases con generación automática de identificador único.
/// </summary>
public class AddClassUseCase : AddUseCase<NewClassDTO, ClassDomain>
{
    /// <summary>
    /// Número máximo de intentos para generar un identificador único.
    /// </summary>
    protected const int maxIdGenerationTries = 20;

    /// <summary>
    /// Servicio para generación de cadenas aleatorias.
    /// </summary>
    protected IRandomStringGeneratorService _idGenerator;

    /// <summary>
    /// Lector para verificar la existencia de clases.
    /// </summary>
    protected IReaderAsync<string, ClassDomain> _reader;

    /// <summary>
    /// Lector para verificar la existencia de usuarios.
    /// </summary>
    protected IReaderAsync<ulong, UserDomain> _usrReader;

    protected ICreatorAsync<ProfessorClassRelationDTO, ProfessorClassRelationDTO> _professorRelationCreator;

    protected List<UserType> _allowedRoles = new List<UserType>() { UserType.PROFESSOR, UserType.ADMIN };

    /// <summary>
    /// Inicializa una nueva instancia de la clase <see cref="AddClassUseCase"/>.
    /// </summary>
    /// <param name="creator">Creador de entidades de clase.</param>
    /// <param name="validator">Validador de reglas de negocio para clases.</param>
    /// <param name="userReader">Lector para verificar la existencia de usuarios.</param>
    /// <param name="reader">Lector para verificar la existencia de clases.</param>
    /// <param name="idGenerator">Servicio para generación de identificadores únicos.</param>
    public AddClassUseCase(
      ICreatorAsync<ClassDomain, NewClassDTO> creator,
      IBusinessValidationService<NewClassDTO> validator,
      IReaderAsync<ulong, UserDomain> userReader,
      IReaderAsync<string, ClassDomain> reader,
      IRandomStringGeneratorService idGenerator,
      ICreatorAsync<ProfessorClassRelationDTO, ProfessorClassRelationDTO> professorRelationCreator
    ) : base(creator, validator)
    {
        _reader = reader;
        _usrReader = userReader;
        _idGenerator = idGenerator;
        _professorRelationCreator = professorRelationCreator;
    }

    protected async override Task<Result<Unit, UseCaseErrorImpl>> ExtraValidationAsync(NewClassDTO value)
    {
        var errors = new List<FieldErrorDTO>();
        var usrSearch = await _usrReader.GetAsync(value.OwnerId);

        if (usrSearch.IsNone)
        {
            errors.Add(new FieldErrorDTO
            {
                Field = "ownerId",
                Message = "No se encontró el usuario"
            });
            return Result.Err(UseCaseError.Input(errors));
        }

        var usr = usrSearch.Unwrap();
        if (!_allowedRoles.Contains(usr.Role))
        {
            return Result.Err(UseCaseError.UnauthorizedError());
        }

        return Result<Unit, UseCaseErrorImpl>.Ok(Unit.Value);
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
        var repeated = Optional<ClassDomain>.None();

        for (int i = 0; i < maxIdGenerationTries; i++)
        {
            id = _idGenerator.Generate();
            repeated = await _reader.GetAsync(id);

            if (repeated.IsNone)
            {
                value.Id = id;
                return value;
            }
        }

        throw new InvalidOperationException($"No se pudo generar un identificador único después de {maxIdGenerationTries} intentos. Es posible que se hayan agotado las combinaciones disponibles.");
    }

    /// <summary>
    /// Crea la relación profesor - clase asignando por defecto al 
    /// al profesor como dueño
    /// </summary>
    /// <param name="newEntity">DTO con el ID del profesor</param>
    /// <param name="createdEntity">Clase creada con ID generado </param>
    protected async override Task ExtraTaskAsync(NewClassDTO newEntity, ClassDomain createdEntity)
    {
        await _professorRelationCreator.AddAsync(new ProfessorClassRelationDTO
        {
            Id = new ClassUserRelationIdDTO { UserId = newEntity.OwnerId, ClassId = createdEntity.Id },
            IsOwner = true
        });
    }
}
