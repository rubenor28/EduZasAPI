namespace EduZasAPI.Application.Ports.DAOs;

using EduZasAPI.Domain.Entities;
using EduZasAPI.Domain.ValueObjects.Common;
using EduZasAPI.Application.DTOs.Users;

/// <summary>
/// Define un repositorio específico para operaciones asíncronas con entidades de usuario.
/// </summary>
/// <remarks>
/// Esta interfaz especializa el repositorio genérico <see cref="IRepositoryAsync{I, E, NE, UE, C}"/>
/// para trabajar específicamente con entidades de tipo <see cref="User"/>, usando <see cref="ulong"/>
/// como identificador y los DTOs específicos para operaciones con usuarios.
/// </remarks>
public interface IUserRepositoryAsync : IRepositoryAsync<ulong, User, NewUserDTO, UserUpdateDTO, UserCriteriaDTO>
{
    /// <summary>
    /// Busca un usuario por su dirección de correo electrónico.
    /// </summary>
    /// <param name="email">Dirección de correo electrónico a buscar.</param>
    /// <returns>
    /// Una tarea que representa la operación asíncrona. El resultado contiene un <see cref="Optional{T}"/>
    /// con el usuario si fue encontrado, o <see cref="Optional{T}.None"/> si no existe ningún usuario con ese email.
    /// </returns>
    Task<Optional<User>> FindByEmail(string email);
}
