using Application.DAOs;
using Application.DTOs.Common;
using Application.DTOs.Tags;
using Application.UseCases.Common;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;

namespace Application.UseCases.Tags;

public sealed class AddTagUseCase(
    ICreatorAsync<TagDomain, NewTagDTO> creator,
    IQuerierAsync<TagDomain, TagCriteriaDTO> querier
) : IUseCaseAsync<NewTagDTO, TagDomain>
{
    private readonly ICreatorAsync<TagDomain, NewTagDTO> _creator = creator;
    private readonly IQuerierAsync<TagDomain, TagCriteriaDTO> _querier = querier;

    public async Task<Result<TagDomain, UseCaseErrorImpl>> ExecuteAsync(NewTagDTO request)
    {
        var formatted = Format(request);

        var tagSearch = await _querier.GetByAsync(
            new()
            {
                Text = new StringQueryDTO
                {
                    Text = formatted.Text,
                    SearchType = StringSearchType.EQ,
                },
            }
        );

        if (tagSearch.Results.Any())
            return tagSearch.Results.First();
    }

    private static NewTagDTO Format(NewTagDTO data)
    {
        data.Text = data.Text.ToUpperInvariant();

        return data;
    }
}
