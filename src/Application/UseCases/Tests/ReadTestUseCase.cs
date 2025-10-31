using Application.DAOs;
using Application.Services;
using Application.UseCases.Common;
using Domain.Entities;

namespace Application.UseCases.Tests;

public sealed class ReadTestUseCase(
    IReaderAsync<ulong, TestDomain> reader,
    IBusinessValidationService<ulong> validator
) : ReadUseCase<ulong, TestDomain>(reader, validator);
