using Application.DTOs.Common;
using Application.DTOs.Users;
using Domain.Entities;
using InterfaceAdapters.Mappers.Common;

namespace InterfaceAdapters.Mappers.Users;

/// <summary>
/// Mapeador para transformar resultados de búsqueda de usuarios de dominio a DTOs.
/// </summary>
public sealed class UserSearchMapper(IMapper<UserDomain, PublicUserDTO> mapper)
    : IMapper<
        PaginatedQuery<UserDomain, UserCriteriaDTO>,
        PaginatedQuery<PublicUserDTO, UserCriteriaDTO>
    >
{
    /// <summary>
    /// Mapea una consulta paginada de usuarios de dominio a una de DTOs públicos.
    /// </summary>
    /// <param name="input">Resultado de la consulta de dominio.</param>
    /// <returns>Resultado de la consulta con DTOs.</returns>
    public PaginatedQuery<PublicUserDTO, UserCriteriaDTO> Map(
        PaginatedQuery<UserDomain, UserCriteriaDTO> input
    ) =>
        new()
        {
            Criteria = input.Criteria,
            Page = input.Page,
            TotalPages = input.TotalPages,
            Results = input.Results.Select(mapper.Map),
        };
}
