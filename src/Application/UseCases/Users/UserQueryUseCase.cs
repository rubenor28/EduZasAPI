using Application.DAOs;
using Application.DTOs.Users;
using Application.UseCases.Common;
using Domain.Entities;

namespace Application.UseCases.Users;

public sealed class UserQueryUseCase(IQuerierAsync<UserDomain, UserCriteriaDTO> querier)
    : QueryUseCase<UserCriteriaDTO, UserDomain>(querier);
