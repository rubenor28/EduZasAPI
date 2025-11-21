using Application.DAOs;
using Application.Services;
using Application.UseCases.Common;
using Domain.Entities;

namespace Application.UseCases.Auth;

public sealed class ReadUserEmailUseCase(
    IReaderAsync<string, UserDomain> reader,
    IBusinessValidationService<string> validator
) : ReadUseCase<string, UserDomain>(reader, validator);
