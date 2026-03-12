using System.Linq.Expressions;
using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.Application.DAOs.Users;

/// <summary>
/// Implementación de lectura de usuarios por email usando EF.
/// </summary>
public class UserEmailEFReader(EduZasDotnetContext ctx, IMapper<User, UserDomain> mapper)
    : EFReader<string, UserDomain, User>(ctx, mapper)
{
    /// <summary>
    /// Obtiene el predicado para filtrar usuarios por su email.
    /// </summary>
    /// <param name="email">Email del usuario.</param>
    /// <returns>Expresión de predicado.</returns>
    protected override Expression<Func<User, bool>> GetIdPredicate(string email) => u => u.Email == email;
}
