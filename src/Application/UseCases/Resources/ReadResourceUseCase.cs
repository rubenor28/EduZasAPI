using Application.DAOs;
using Application.UseCases.Common;
using Domain.Entities;

namespace Application.UseCases.Resources;

public sealed class ReadResourceUseCase(IReaderAsync<Guid, ResourceDomain> reader)
    : ReadUseCase<Guid, ResourceDomain>(reader, null);
