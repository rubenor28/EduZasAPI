using Application.DAOs;
using Application.Services.Validators;
using Application.UseCases.Common;
using Domain.Entities;

namespace Application.UseCases.Auth;

/// <summary>
/// Caso de uso para verificar la existencia de un usuario por email.
/// </summary>

public sealed class ReadUserEmailUseCase(
    IReaderAsync<string, UserDomain> reader,
    IBusinessValidationService<string> validator
) : ReadUseCase<string, UserDomain>(reader, validator);
