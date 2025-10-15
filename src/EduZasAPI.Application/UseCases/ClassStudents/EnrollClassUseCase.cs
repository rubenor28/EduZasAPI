using EduZasAPI.Domain.Common;
using EduZasAPI.Domain.Users;
using EduZasAPI.Domain.Classes;
using EduZasAPI.Application.Common;

namespace EduZasAPI.Application.Classes;

/// <summary>
/// Implementa el caso de uso para añadir un usuario a una clase.
/// Utiliza el modelo de programación asincrónica (TAP) para la validación de dependencias.
/// </summary>
public class EnrollClassUseCase : AddUseCase<StudentClassRelationDTO, StudentClassRelationDTO>
{
    /// <summary>
    /// Lector asincrónico para acceder a los datos de dominio del usuario (<see cref="UserDomain"/>)
    /// utilizando un identificador de tipo <see cref="ulong"/>.
    /// </summary>
    protected IReaderAsync<ulong, UserDomain> _usrReader;

    /// <summary>
    /// Lector asincrónico para acceder a los datos de dominio de la clase (<see cref="ClassDomain"/>)
    /// utilizando un identificador de tipo <see cref="string"/>.
    /// </summary>
    protected IReaderAsync<string, ClassDomain> _classReader;

    protected IReaderAsync<ClassUserRelationIdDTO, StudentClassRelationDTO> _studentReader;

    protected IReaderAsync<ClassUserRelationIdDTO, ProfessorClassRelationDTO> _professorReader;

    public EnrollClassUseCase(
        IReaderAsync<ulong, UserDomain> userReader,
        IReaderAsync<string, ClassDomain> classReader,
        IReaderAsync<ClassUserRelationIdDTO, StudentClassRelationDTO> studentReader,
        IReaderAsync<ClassUserRelationIdDTO, ProfessorClassRelationDTO> professorReader,
        ICreatorAsync<StudentClassRelationDTO, StudentClassRelationDTO> creator) : base(creator)
    {
        _usrReader = userReader;
        _classReader = classReader;
        _studentReader = studentReader;
        _professorReader = professorReader;
    }

    protected override StudentClassRelationDTO PostValidationFormat(StudentClassRelationDTO value) => new StudentClassRelationDTO
    {
        Id = value.Id,
        Hidden = true
    };

    /// <summary>
    /// Realiza validaciones asincrónicas antes de proceder con la adición de la relación.
    /// Las validaciones incluyen la existencia de la clase y la existencia del usuario.
    /// Las búsquedas de usuario y clase se ejecutan de forma concurrente.
    /// </summary>
    /// <param name="value">El DTO que contiene el ID de usuario y el ID de clase.</param>
    /// <returns>
    /// Un <see cref="Result{TSuccess, TFailure}"/> que indica si la validación fue exitosa 
    /// (<see cref="Unit.Value"/>) o si contiene una lista de errores de campo (<see cref="FieldErrorDTO"/>).
    /// </returns>
    protected async override Task<Result<Unit, UseCaseErrorImpl>> ExtraValidationAsync(
        StudentClassRelationDTO value)
    {
        var relationSearch = await _studentReader.GetAsync(value.Id);

        if (relationSearch.IsSome)
            return Result.Err(UseCaseError.Input(new List<FieldErrorDTO> {
                new FieldErrorDTO { Field = "userId, classId", Message = "El usuario ya se encuentra inscrito a esta clase" }
          }));

        var errors = new List<FieldErrorDTO>();

        var classSearch = await _classReader.GetAsync(value.Id.ClassId);
        if (classSearch.IsNone)
        {
            errors.Add(new FieldErrorDTO
            {
                Field = "classId",
                Message = "Clase no encontrada"
            });
        }

        var usrSearch = await _usrReader.GetAsync(value.Id.UserId);
        usrSearch.IfNone(() => errors.Add(new FieldErrorDTO
        {
            Field = "userId",
            Message = "Usuario no encontrado"
        }));

        var userIsProfessorSearch = await _professorReader.GetAsync(value.Id);
        userIsProfessorSearch.IfSome(_ => errors.Add(new FieldErrorDTO
        {
            Field = "userId",
            Message = "El usuario ya es profesor de la clase"
        }));

        if (errors.Count > 0)
            return Result.Err(UseCaseError.Input(errors));

        return Result<Unit, UseCaseErrorImpl>.Ok(Unit.Value);
    }
}
