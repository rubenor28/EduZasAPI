using Application.DAOs;
using Application.DTOs.Tags;
using Application.UseCases.Common;
using Domain.Entities;

namespace Application.UseCases.Tags;

/// <summary>
/// Caso de uso para consultar etiquetas.
/// </summary>
public sealed class TagQueryUseCase(IQuerierAsync<TagDomain, TagCriteriaDTO> querier)
    : QueryUseCase<TagCriteriaDTO, TagDomain>(querier);
