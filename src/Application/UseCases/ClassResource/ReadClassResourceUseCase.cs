using Application.DAOs;
using Application.DTOs.ClassResources;
using Application.Services;
using Application.UseCases.Common;
using Domain.Entities;

namespace Application.UseCases.ClassResource;

public sealed class ReadClassResourceUseCase(
    IReaderAsync<ClassResourceIdDTO, ClassResourceDomain> reader,
    IBusinessValidationService<ClassResourceIdDTO>? validator = null
) : ReadUseCase<ClassResourceIdDTO, ClassResourceDomain>(reader, validator);
