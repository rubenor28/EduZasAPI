using Application.DTOs.Common;
using Domain.ValueObjects;

namespace Application.DTOs.Contacts;

public sealed record DeleteContactDTO(ulong Id, Executor Executor) : IIdentifiable<ulong>;
