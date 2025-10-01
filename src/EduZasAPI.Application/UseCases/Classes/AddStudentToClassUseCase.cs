using EduZasAPI.Domain.Common;
using EduZasAPI.Domain.Users;
using EduZasAPI.Domain.Classes;
using EduZasAPI.Application.Common;

namespace EduZasAPI.Application.Classes;

/// <summary>
/// Implementa el caso de uso para añadir un usuario a una clase.
/// Utiliza el modelo de programación asincrónica (TAP) para la validación de dependencias.
/// </summary>
public class AddStudentToClassUseCase : AddUseCase<StudentClassRelationDTO,StudentClassRelationDTO>
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

    public AddStudentToClassUseCase(
        IReaderAsync<ulong, UserDomain> userReader,
        IReaderAsync<string, ClassDomain> classReader,
        ICreatorAsync<StudentClassRelationDTO, StudentClassRelationDTO> creator) : base(creator)
    {
        _usrReader = userReader;
        _classReader = classReader;
    }

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
    protected async override Task<Result<Unit, List<FieldErrorDTO>>> ExtraValidationAsync(
        StudentClassRelationDTO value)
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

        usrSearch.IfNone(() => errors.Add(new FieldErrorDTO
        {
            Field = "userId",
            Message = "Usuario no encontrado"
        }));

        if (errors.Count > 0) return Result.Err(errors);

        return Result<Unit, List<FieldErrorDTO>>.Ok(Unit.Value);
    }
}
