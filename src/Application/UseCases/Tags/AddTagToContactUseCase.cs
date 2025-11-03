using Application.DAOs;
using Application.DTOs.Common;
using Application.DTOs.Contacts;
using Application.DTOs.ContactTags;
using Application.UseCases.Common;
using Domain.Entities;
using Domain.ValueObjects;

namespace Application.UseCases.Tags;

using ContactQuerier = IQuerierAsync<ContactDomain, ContactCriteriaDTO>;

public sealed class AddTagToContactUseCase(
    ContactQuerier contactQuerier
    ) : IUseCaseAsync<NewContactDTO, Unit>
{
    private readonly ContactQuerier _contactQuerier = contactQuerier;

    public Task<Result<Unit, UseCaseErrorImpl>> ExecuteAsync(NewContactDTO request)
    {
        throw new NotImplementedException();
    }
}
