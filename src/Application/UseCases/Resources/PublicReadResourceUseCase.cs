using Application.DAOs;
using Application.DTOs.Resources;
using Application.UseCases.Common;
using Domain.Entities;

namespace Application.UseCases.Resources;

/// <summary>
/// Caso de uso para leer un recurso por su ID.
/// </summary>
public sealed class ReadResourceUseCase
    : ReadUseCase<ReadResourceDTO, ResourceDomain> {

    };
