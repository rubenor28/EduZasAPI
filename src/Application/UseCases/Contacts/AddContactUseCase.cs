using Application.DAOs;
using Application.DTOs.Common;
using Application.DTOs.Contacts;
using Application.UseCases.Common;
using Application.UseCases.Tags;
using Domain.Entities;
using Domain.ValueObjects;

namespace Application.UseCases.Contacts;

// public sealed class AddContactUseCasse(
//     ICreatorAsync<NewContactDTO, ContactDomain> contactCreator,
//     IQuerierAsync<ContactDomain, ContactCriteriaDTO> contactQuerier,
//     AddTagUseCase addTagUseCase
//     ) : IUseCaseAsync<NewContactUseCaseDTO, ContactDomain>
// {
//     private readonly ICreatorAsync<NewContactDTO, ContactDomain> _contactCreator = contactCreator;
//     private readonly IQuerierAsync<ContactDomain, ContactCriteriaDTO> _contactQuerier = contactQuerier;
//     private readonly AddTagUseCase _addTagUseCase = addTagUseCase;
//
//     public async Task<Result<ContactDomain, UseCaseErrorImpl>> ExecuteAsync(NewContactUseCaseDTO request)
//     {
//       var userExistenceSearch = await _contactQuerier.GetByAsync(new() {
//           Id
//           })
//     }
// }
