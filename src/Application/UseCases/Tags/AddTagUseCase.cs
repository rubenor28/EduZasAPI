using Application.DAOs;
using Application.DTOs.Common;
using Application.DTOs.Tags;
using Application.Services;
using Application.UseCases.Common;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;

namespace Application.UseCases.Tags;

public sealed class AddTagUseCase(
    ICreatorAsync<TagDomain, NewTagDTO> creator,
    IQuerierAsync<TagDomain, TagCriteriaDTO> querier,
    IBusinessValidationService<NewTagDTO>? validator = null
) : AddUseCase<NewTagDTO, TagDomain>(creator, validator)
{
    private readonly IQuerierAsync<TagDomain, TagCriteriaDTO> _querier = querier;

    protected override NewTagDTO PreValidationFormat(NewTagDTO data)
    {
        data.Text = data.Text.ToUpperInvariant();
        return data;
    }

    protected override async Task<Result<Unit, UseCaseErrorImpl>> ExtraValidationAsync(
        NewTagDTO value
    )
    {
        var tagSearch = await _querier.GetByAsync(
            new()
            {
                Text = new StringQueryDTO { Text = value.Text, SearchType = StringSearchType.EQ },
            }
        );

        if (tagSearch.Results.Any())
            return UseCaseError.AlreadyExists();

        return Unit.Value;
    }
}
