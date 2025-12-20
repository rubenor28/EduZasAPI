using Application.DAOs;
using Application.UseCases.Common;
using Domain.Entities;

namespace Application.UseCases.Tests;

/// <summary>
/// Caso de uso para leer una evaluación por su ID.
/// </summary>
public sealed class ReadTestUseCase(IReaderAsync<Guid, TestDomain> reader)
    : ReadUseCase<Guid, TestDomain>(reader, null) { }
