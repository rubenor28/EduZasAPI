using Application.DAOs;
using Application.UseCases.Common;
using Domain.Entities;

namespace Application.UseCases.Tests;

/// <summary>
/// Caso de uso para eliminar una evaluación.
/// </summary>
public sealed class DeleteTestUseCase(
    IDeleterAsync<Guid, TestDomain> deleter,
    IReaderAsync<Guid, TestDomain> reader
) : DeleteUseCase<Guid, TestDomain>(deleter, reader, null)
{
    // TODO: Eliminar las respuestas asociadas a una evaluacion
    // TODO: Eliminar asociaciones clase, test
}
