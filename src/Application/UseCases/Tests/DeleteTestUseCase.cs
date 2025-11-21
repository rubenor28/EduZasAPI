using Application.DAOs;
using Application.DTOs.Tests;
using Application.Services;
using Application.UseCases.Common;
using Domain.Entities;

namespace Application.UseCases.Tests;

public sealed class DeleteTestUseCase(
    IDeleterAsync<ulong, TestDomain> deleter,
    IReaderAsync<ulong, TestDomain> reader,
    IBusinessValidationService<DeleteTestDTO>? validator = null
) : DeleteUseCase<ulong, DeleteTestDTO, TestDomain>(deleter, reader, validator)
{
    // TODO: Eliminar las respuestas asociadas a una evaluacion
    // TODO: Eliminar asociaciones clase, test
    protected override ulong GetId(DeleteTestDTO value) => value.Id;

    protected override ulong GetId(TestDomain value) => value.Id;
}
