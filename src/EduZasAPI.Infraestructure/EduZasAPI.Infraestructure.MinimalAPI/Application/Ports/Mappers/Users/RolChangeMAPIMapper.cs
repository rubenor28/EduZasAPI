using EduZasAPI.Application.Users;

namespace EduZasAPI.Infraestructure.MinimalAPI.Application.Users;

/// <summary>
/// Métodos de extensión para convertir <see cref="RolChangeMAPI"/> a objetos de dominio.
/// </summary>
public static class RolChangeMAPIMapper
{
    /// <summary>
    /// Convierte una instancia de <see cref="RolChangeMAPI"/> en un <see cref="RolChangeDTO"/>.
    /// </summary>
    /// <param name="value">Instancia de <see cref="RolChangeMAPI"/> a convertir.</param>
    /// <returns>
    /// Un objeto <see cref="RolChangeDTO"/> con el identificador y el rol
    /// mapeados al modelo de dominio.  
    /// Si el rol no se puede convertir, se asigna el valor por defecto <c>0</c>.
    /// </returns>
    public static RolChangeDTO ToDomain(this RolChangeMAPI value) => new RolChangeDTO
    {
        Id = value.Id,
        Role = UserTypeMapper.FromInt(value.Role).UnwrapOr(0),
    };
}
