using Application.DAOs;
using Application.Services;
using Application.UseCases.Common;
using Domain.Entities;

namespace Application.UseCases.Resources;

public sealed class ReadResourceUseCase(
    IReaderAsync<ulong, ResourceDomain> reader,
    IBusinessValidationService<ulong> validator
) : ReadUseCase<ulong, ResourceDomain>(reader, validator);
