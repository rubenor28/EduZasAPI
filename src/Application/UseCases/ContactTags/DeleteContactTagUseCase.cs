using Application.DAOs;
using Application.DTOs.ContactTags;
using Application.Services;
using Application.UseCases.Common;
using Domain.Entities;

namespace Application.UseCases.ContactTags;

public sealed class DeleteContactTagUseCase(
    IDeleterAsync<ContactTagIdDTO, ContactTagDomain> deleter,
    IReaderAsync<ContactTagIdDTO, ContactTagDomain> reader,
    IBusinessValidationService<DeleteContactTagDTO>? validator = null
)
    : DeleteUseCase<ContactTagIdDTO, DeleteContactTagDTO, ContactTagDomain>(
        deleter,
        reader,
        validator
    );
