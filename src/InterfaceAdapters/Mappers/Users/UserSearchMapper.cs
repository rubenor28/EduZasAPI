using Application.DTOs.Common;
using Application.DTOs.Users;
using Domain.Entities;
using InterfaceAdapters.Mappers.Common;

namespace InterfaceAdapters.Mappers.Users;

public sealed class UserSearchMapper(IMapper<UserDomain, PublicUserDTO> mapper)
    : IMapper<
        PaginatedQuery<UserDomain, UserCriteriaDTO>,
        PaginatedQuery<PublicUserDTO, UserCriteriaDTO>
    >
{
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
