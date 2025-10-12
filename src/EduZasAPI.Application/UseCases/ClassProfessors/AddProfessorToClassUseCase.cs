using EduZasAPI.Domain.Common;
using EduZasAPI.Domain.Users;
using EduZasAPI.Domain.Classes;
using EduZasAPI.Application.Common;
using EduZasAPI.Application.Classes;

namespace EduZasAPI.Application.ProfessorClasses;

/// <summary>
/// Implementa el caso de uso para añadir un usuario a una clase, validando
/// que el usuario posea los permisos requeridos (Professor o Administrador).
/// Utiliza el modelo de programación asincrónica (TAP) para la validación de dependencias.
/// </summary>
public class AddProfessorToClassUseCase : AddUseCase<ProfessorClassRelationDTO, ProfessorClassRelationDTO>
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

    /// <summary>
    /// Lista de roles de usuario permitidos para ser añadidos a la clase.
    /// Solo se admiten <see cref="UserType.PROFESSOR"/> y <see cref="UserType.ADMIN"/>.
    /// </summary>
    protected readonly List<UserType> _admitedRoles = new List<UserType>() {
      UserType.PROFESSOR,
      UserType.ADMIN
    };

    /// <summary>
    /// Inicializa una nueva instancia de la clase <see cref="AddProfessorToClassUseCase"/>.
    /// </summary>
    /// <param name="userReader">Servicio para la lectura asincrónica de usuarios.</param>
    /// <param name="classReader">Servicio para la lectura asincrónica de clases.</param>
    /// <param name="creator">Servicio para la creación de la relación UserToClass, pasado a la base.</param>
    public AddProfessorToClassUseCase(
        IReaderAsync<ulong, UserDomain> userReader,
        IReaderAsync<string, ClassDomain> classReader,
        ICreatorAsync<ProfessorClassRelationDTO, ProfessorClassRelationDTO> creator) : base(creator)
    {
        _usrReader = userReader;
        _classReader = classReader;
    }

    /// <summary>
    /// Realiza validaciones asincrónicas antes de proceder con la adición de la relación.
    /// Las validaciones incluyen la existencia de la clase, la existencia del usuario,
    /// y la verificación de que el usuario tenga un rol permitido.
    /// Las búsquedas de usuario y clase se ejecutan de forma concurrente.
    /// </summary>
    /// <param name="value">El DTO que contiene el ID de usuario y el ID de clase.</param>
    /// <returns>
    /// Un <see cref="Result{TSuccess, TFailure}"/> que indica si la validación fue exitosa 
    /// (<see cref="Unit.Value"/>) o si contiene una lista de errores de campo (<see cref="FieldErrorDTO"/>).
    /// </returns>
    protected async override Task<Result<Unit, UseCaseErrorImpl>> ExtraValidationAsync(
        ProfessorClassRelationDTO value)
    {
        var usrSearchTask = _usrReader.GetAsync(value.Id.UserId);
        var classSearchTask = _classReader.GetAsync(value.Id.ClassId);
        var errors = new List<FieldErrorDTO>();

        if ((await classSearchTask).IsNone)
        {
            errors.Add(new FieldErrorDTO
            {
                Field = "classId",
                Message = "Clase no encontrada"
            });
        }

        var usrSearch = await usrSearchTask;

        usrSearch.IfSome(usr =>
        {
            if (!_admitedRoles.Contains(usr.Role))
            {
                errors.Add(new FieldErrorDTO
                {
                    Field = "userId",
                    Message = "Permisos inadecuados"
                });
            }
        });

        usrSearch.IfNone(() => errors.Add(new FieldErrorDTO
        {
            Field = "userId",
            Message = "Usuario no encontrado"
        }));

        if (errors.Count > 0) return Result.Err(UseCaseError.InputError(errors));

        return Result<Unit, UseCaseErrorImpl>.Ok(Unit.Value);
    }
}
