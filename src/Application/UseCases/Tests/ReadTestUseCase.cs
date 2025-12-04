using Application.DAOs;
using Application.UseCases.Common;
using Domain.Entities;

namespace Application.UseCases.Tests;

public sealed class ReadTestUseCase(
    IReaderAsync<Guid, TestDomain> reader
) : ReadUseCase<Guid, TestDomain>(reader, null);
