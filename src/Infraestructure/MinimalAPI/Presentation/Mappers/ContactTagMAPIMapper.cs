using Application.DTOs.Common;
using Application.DTOs.ContactTags;
using Domain.Entities;
using InterfaceAdapters.Mappers.Common;

namespace MinimalAPI.Presentation.Mappers;

public sealed class ContactTagMAPIMapper
    : IMapper<ContactTagIdDTO, Executor, NewContactTagDTO>,
        IMapper<ulong, ulong, string, Executor, DeleteContactTagDTO>
{
    public DeleteContactTagDTO Map(ulong agendaOwnerId, ulong userId, string tag, Executor ex) =>
        new()
        {
            Id = new()
            {
                UserId = userId,
                AgendaOwnerId = agendaOwnerId,
                Tag = tag,
            },
            Executor = ex,
        };

    NewContactTagDTO IMapper<ContactTagIdDTO, Executor, NewContactTagDTO>.Map(
        ContactTagIdDTO ct,
        Executor ex
    ) =>
        new()
        {
            UserId = ct.UserId,
            AgendaOwnerId = ct.AgendaOwnerId,
            Tag = ct.Tag,
            Executor = ex,
        };
}
