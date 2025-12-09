using Application.DAOs;
using Application.DTOs.Contacts;
using Application.UseCases.Common;
using Domain.Entities;

namespace Application.UseCases.Contacts;

/// <summary>
/// Caso de uso para consultar contactos.
/// </summary>
public sealed class ContactQueryUseCase(IQuerierAsync<ContactDomain, ContactCriteriaDTO> querier)
    : QueryUseCase<ContactCriteriaDTO, ContactDomain>(querier);
