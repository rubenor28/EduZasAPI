using Application.DAOs;
using Application.DTOs.Contacts;
using Application.Services;
using Application.UseCases.Common;
using Domain.Entities;

namespace Application.UseCases.Contacts;

public sealed class UpdateContactUseCase(
    IUpdaterAsync<ContactDomain, ContactUpdateDTO> updater,
    IBusinessValidationService<ContactUpdateDTO>? validator = null
) : UpdateUseCase<ContactUpdateDTO, ContactDomain>(updater, validator) { }
