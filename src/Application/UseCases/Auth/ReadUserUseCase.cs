using Application.DAOs;
using Application.DTOs.Users;
using Application.Services;
using Application.UseCases.Common;
using Domain.Entities;

namespace Application.UseCases.Auth;

public sealed class ReadUserUseCase(
    IReaderAsync<ulong, UserDomain> reader,
    IBusinessValidationService<ulong> validator
) : ReadUseCase<ulong, ReadUserDTO, UserDomain>(reader, validator);
