using Application.DAOs;
using Application.DTOs.ClassResources;
using Application.Services.Validators;
using Application.UseCases.Common;
using Domain.Entities;

namespace Application.UseCases.ClassResource;

/// <summary>
/// Caso de uso para leer la asociación de un recurso con una clase.
/// </summary>
public sealed class ReadClassResourceUseCase(
    IReaderAsync<ClassResourceIdDTO, ClassResourceDomain> reader,
    IBusinessValidationService<ClassResourceIdDTO>? validator = null
) : ReadUseCase<ClassResourceIdDTO, ClassResourceDomain>(reader, validator);
