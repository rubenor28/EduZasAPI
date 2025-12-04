using System.Threading.Tasks;
using Application.DTOs;
using Application.DTOs.Common;
using Domain.ValueObjects;

namespace Application.UseCases.Common;

public interface IUseCaseAsync<TRequest, TResponse>
    where TRequest : notnull
    where TResponse : notnull
{
    Task<Result<TResponse, UseCaseError>> ExecuteAsync(UserActionDTO<TRequest> request);
}

public interface IGuestUseCaseAsync<TRequest, TResponse>
    where TRequest : notnull
    where TResponse : notnull
{
    Task<Result<TResponse, UseCaseError>> ExecuteAsync(TRequest request);
}
