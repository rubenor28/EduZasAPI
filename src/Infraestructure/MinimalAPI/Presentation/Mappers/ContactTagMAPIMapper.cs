using Application.DTOs.ContactTags;
using Domain.Entities;
using InterfaceAdapters.Mappers.Common;

namespace MinimalAPI.Presentation.Mappers;

public sealed class NewContactTagMAPIMapper
    : IMapper<ContactTagIdDTO, NewContactTagDTO>
{
    public NewContactTagDTO Map(ContactTagIdDTO ct) =>
        new()
        {
            UserId = ct.UserId,
            AgendaOwnerId = ct.AgendaOwnerId,
            Tag = ct.Tag,
        };
}
