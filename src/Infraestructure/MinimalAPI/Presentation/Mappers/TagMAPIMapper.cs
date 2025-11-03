using Application.DTOs.Common;
using Application.DTOs.Tags;
using Domain.Entities;
using Domain.ValueObjects;
using InterfaceAdapters.Mappers.Common;
using MinimalAPI.Application.DTOs.Common;
using MinimalAPI.Application.DTOs.Tags;

namespace MinimalAPI.Presentation.Mappers;

public static class TagMAPIMapper
{
    public static Result<TagCriteriaDTO, UseCaseErrorImpl> ToDomain(this TagCriteriaMAPI source)
    {
        var invalidFormat = source.Text is not null && source.Text.ToDomain().IsErr;

        if (invalidFormat)
            return UseCaseError.Input([new() { Field = "text", Message = "Formato invalido" }]);

        return new TagCriteriaDTO
        {
            Text = source.Text switch
            {
                not null => source.Text.ToDomain().Unwrap(),
                null => Optional<StringQueryDTO>.None(),
            },
            ContactId = source.ContactId.ToOptional(),
            AgendaOwnerId = source.AgendaOwnerId.ToOptional(),
            Page = source.Page,
        };
    }

    public static TagCriteriaMAPI FromDomain(this TagCriteriaDTO source) =>
        new()
        {
            Page = source.Page,
            ContactId = source.ContactId.ToNullable(),
            AgendaOwnerId = source.AgendaOwnerId.ToNullable(),
            Text = source.Text.Match<StringQueryMAPI?>((value) => value.FromDomain(), () => null),
        };

    public static PublicTagMAPI FromDomain(this TagDomain source) => new() { Text = source.Text };
}
