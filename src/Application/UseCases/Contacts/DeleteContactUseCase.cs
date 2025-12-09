using System.Buffers;
using System.Text;
using Application.DAOs;
using Application.DTOs;
using Application.DTOs.Common;
using Application.DTOs.Contacts;
using Application.DTOs.Tags;
using Application.UseCases.Common;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;

namespace Application.UseCases.Contacts;

using ContactDeleter = IDeleterAsync<ContactIdDTO, ContactDomain>;
using ContactQuerier = IQuerierAsync<ContactDomain, ContactCriteriaDTO>;
using ContactReader = IReaderAsync<ContactIdDTO, ContactDomain>;
using ContactTagDeleter = IDeleterAsync<ContactTagIdDTO, ContactTagDomain>;
using TagDeleter = IDeleterAsync<string, TagDomain>;
using TagQuerier = IQuerierAsync<TagDomain, TagCriteriaDTO>;

/// <summary>
/// Caso de uso para eliminar un contacto y limpiar etiquetas huérfanas.
/// </summary>
public sealed class DeleteContactUseCase(
    ContactDeleter deleter,
    ContactReader reader,
    TagQuerier tagQuerier,
    TagDeleter tagDeleter,
    ContactQuerier contactQuerier,
    ContactTagDeleter contactTagDeleter
) : DeleteUseCase<ContactIdDTO, ContactDomain>(deleter, reader, null)
{
    private readonly ContactQuerier _contactQuerier = contactQuerier;
    private readonly ContactTagDeleter _contactTagDeleter = contactTagDeleter;

    private readonly TagQuerier _tagQuerier = tagQuerier;
    private readonly TagDeleter _tagDeleter = tagDeleter;

    /// <inheritdoc/>
    protected override async Task<Result<Unit, UseCaseError>> ExtraValidationAsync(
        UserActionDTO<ContactIdDTO> value,
        ContactDomain record
    )
    {
        var authorized = value.Executor.Role switch
        {
            UserType.ADMIN => true,
            UserType.PROFESSOR => record.Id.AgendaOwnerId == value.Executor.Id,
            UserType.STUDENT => false,
            _ => throw new NotImplementedException(),
        };

        if (!authorized)
            return UseCaseErrors.Unauthorized();

        return Unit.Value;
    }

    /// <inheritdoc/>
    protected override async Task PrevTaskAsync(
        UserActionDTO<ContactIdDTO> deleteDTO,
        ContactDomain record
    )
    {
        // Proveedor de arrays alojados en el stack para evitar creacion de
        // arrays en el heap de forma inecesaria (reducir carga del Garbage Collector)
        var arrProvider = ArrayPool<string>.Shared;

        int currPage = 0;
        List<string> tags = [];
        PaginatedQuery<TagDomain, TagCriteriaDTO> contactTags;

        // Obtener todas las etiquetas del contacto a eliminar
        do
        {
            contactTags = await _tagQuerier.GetByAsync(new() { ContactId = deleteDTO.Data.UserId });

            var listOfTagText = contactTags.Results.Select(t => t.Text);
            tags.AddRange(listOfTagText);

            currPage += 1;
        } while (currPage <= contactTags.TotalPages);

        // Cadena que representan los errores que podriamos tener
        StringBuilder errStr = new();
        // Flag para indicar si hubo un error
        var error = false;

        foreach (var tag in tags)
        {
            try
            {
                // Eliminar relacion entre etiqueta y contacto a eliminar
                await _contactTagDeleter.DeleteAsync(
                    new()
                    {
                        Tag = tag,
                        AgendaOwnerId = deleteDTO.Data.AgendaOwnerId,
                        UserId = deleteDTO.Data.UserId,
                    }
                );

                string[] tagArr = null!;
                try
                {
                    // Solicitar array para 1 elemento
                    tagArr = arrProvider.Rent(1);
                    // Colocar etiqueta a buscar
                    tagArr[0] = tag;

                    // Buscar otros contactos que usen la etiqueta
                    var contactsHaveTag = (
                        await _contactQuerier.GetByAsync(new() { Tags = tagArr })
                    ).Results.Any();

                    // Si ningun otro contacto usa la etiqueta, la eliminamos
                    if (!contactsHaveTag)
                        await _tagDeleter.DeleteAsync(tag);
                }
                catch (Exception)
                {
                    error = true;
                    errStr.Append($"Error eliminando etiqueta: {tag}\n");
                }
                finally
                {
                    // Devolver array al pool
                    if (tagArr is not null)
                        arrProvider.Return(tagArr);
                }
            }
            catch (Exception)
            {
                error = true;
                errStr.Append(
                    $"Error eliminadno relacion usuario-etiqueta: ({deleteDTO.Data.UserId},{tag})\n"
                );
            }
        }

        if (error)
            throw new Exception(errStr.ToString());
    }
}
