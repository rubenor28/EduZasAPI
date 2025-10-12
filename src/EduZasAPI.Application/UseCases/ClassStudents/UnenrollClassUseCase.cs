using EduZasAPI.Domain.Common;
using EduZasAPI.Domain.Users;
using EduZasAPI.Domain.Classes;
using EduZasAPI.Application.Common;

namespace EduZasAPI.Application.Classes;

/// <summary>
/// Implementa el caso de uso para añadir un usuario a una clase.
/// Utiliza el modelo de programación asincrónica (TAP) para la validación de dependencias.
/// </summary>
public class UnEnrollClassUseCase : DeleteUseCase<ClassUserRelationIdDTO, UnenrollClassDTO, StudentClassRelationDTO>
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
    /// Lector asíncrono para acceder a las relaciones entre clases y estudiantes (<see cref="StudentClassRelationDTO"/>)
    /// utilizando un identificador de tipo <see cref="ClassUserRelationIdDTO" />
    /// </summary>
    protected IReaderAsync<ClassUserRelationIdDTO, StudentClassRelationDTO> _studentRelationReader;

    /// <summary>
    /// Lector asíncrono para acceder a las relaciones entre clases y profesores (<see cref="ProfessorClassRelationDTO"/>)
    /// utilizando un identificador de tipo <see cref="ClassUserRelationIdDTO" />
    /// </summary>
    protected IReaderAsync<ClassUserRelationIdDTO, ProfessorClassRelationDTO> _professorRelationReader;

    public UnEnrollClassUseCase(
        IReaderAsync<ulong, UserDomain> userReader,
        IReaderAsync<string, ClassDomain> classReader,
        IDeleterAsync<ClassUserRelationIdDTO, StudentClassRelationDTO> deleter,
        IReaderAsync<ClassUserRelationIdDTO, ProfessorClassRelationDTO> professorRelationReader,
        IReaderAsync<ClassUserRelationIdDTO, StudentClassRelationDTO> studentRelationReader) : base(deleter)
    {
        _usrReader = userReader;
        _classReader = classReader;
        _studentRelationReader = studentRelationReader;
        _professorRelationReader = professorRelationReader;
    }


    /// <summary>
    /// Realiza validaciones asincrónicas antes de proceder con la adición de la relación.
    /// Las validaciones incluyen la existencia de la clase, la existencia del usuario y 
    /// la existencia de la relación.
    /// </summary>
    /// <param name="value">El DTO que contiene el ID de usuario y el ID de clase.</param>
    /// <returns>
    /// Un <see cref="Result{TSuccess, TFailure}"/> que indica si la validación fue exitosa 
    /// (<see cref="Unit.Value"/>) o si contiene una lista de errores de campo (<see cref="FieldErrorDTO"/>).
    /// </returns>
    protected async override Task<Result<Unit, UseCaseErrorImpl>> ExtraValidationAsync(UnenrollClassDTO value)
    {
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

        var studentRelationSearch = await _studentRelationReader.GetAsync(value.Id);

        switch (studentRelationSearch)
        {
            // Si no se encuentra la relacion, se marca como error de la entrada
            case { IsNone: true }:
                {
                    errors.Add(new FieldErrorDTO { Field = "userId, classId", Message = "No se encontró la relacion" });
                    break;
                }

            /**
             * Validar que el ejecutor sea, o administrador, o profesor dueño de la clase.
             * Se hace uso del early return. Un break indica un caso valido, y en caso de
             * return envia un error o una excepcion según corresponda
             */
            case { IsSome: true }:
                {
                    var relation = studentRelationSearch.Unwrap();

                    if (relation.Id.UserId == value.Executor.Id)
                        break;

                    if (value.Executor.Role == UserType.ADMIN)
                        break;

                    if (value.Executor.Role == UserType.STUDENT)
                        return Result.Err(UseCaseError.UnauthorizedError());

                    var professorRelationSearch = await _professorRelationReader.GetAsync(new ClassUserRelationIdDTO
                    {
                        UserId = value.Executor.Id,
                        ClassId = value.Id.ClassId
                    });

                    // Caso el argumento diga que el ejecutor es usuario pero los registros
                    // digan lo contrario
                    if (professorRelationSearch.IsNone)
                        throw new InvalidOperationException("El executor dice ser profesor pero no lo es");

                    var professorRelation = professorRelationSearch.Unwrap();

                    if (professorRelation.IsOwner)
                        break;

                    return Result.Err(UseCaseError.UnauthorizedError());
                }
        }

        if (errors.Count > 0)
            return Result.Err(UseCaseError.InputError(errors));

        return Result<Unit, UseCaseErrorImpl>.Ok(Unit.Value);
    }
}
