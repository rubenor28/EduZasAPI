using Application.DAOs;
using Application.DTOs.Contacts;
using Application.Services;
using Application.UseCases.Common;
using Domain.Entities;

namespace Application.UseCases.Contacts;

public sealed class UpdateContactUseCase(
    IUpdaterAsync<ContactDomain, ContactUpdateDTO> updater,
    IReaderAsync<ulong, ContactDomain> reader,
    IBusinessValidationService<ContactUpdateDTO>? validator = null
) : UpdateUseCase<ulong, ContactUpdateDTO, ContactDomain>(updater, reader, validator) { }
