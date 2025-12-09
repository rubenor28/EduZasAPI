using Application.DAOs;
using Application.UseCases.Common;
using Domain.Entities;

namespace Application.UseCases.Resources;

/// <summary>
/// Caso de uso para leer un recurso por su ID.
/// </summary>
public sealed class ReadResourceUseCase(IReaderAsync<Guid, ResourceDomain> reader)
    : ReadUseCase<Guid, ResourceDomain>(reader, null);
