using EduZasAPI.Domain.Common;
using EduZasAPI.Domain.Classes;
using EduZasAPI.Application.Common;

namespace EduZasAPI.Application.Classes;

/// <inheritdoc/>
public class AddClassUseCase : AddUseCase<NewClassDTO, ClassDomain>
{
    protected const int maxTries = 20;
    protected IRandomStringGeneratorService _idGenerator;
    protected IReaderAsync<string, ClassDomain> _reader;
    protected RandomStringGeneratorArgs _cfg;

    public AddClassUseCase(
      ICreatorAsync<ClassDomain, NewClassDTO> creator,
      IBusinessValidationService<NewClassDTO> validator,
      IReaderAsync<string, ClassDomain> reader,
      IRandomStringGeneratorService idGenerator
    ) : base(creator, validator)
    {
        _reader = reader;
        _idGenerator = idGenerator;
        _cfg = new RandomStringGeneratorArgs
        {
            MaxStrLenght = 20,
            AllowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray()
        };
    }

    /// <inheritdoc/>
    protected async override Task<NewClassDTO> PostValidationFormatAsync(NewClassDTO value)
    {
        string id;
        var repeated = Optional<ClassDomain>.None();

        for (int i = 0; i < maxTries; i++)
        {
            id = _idGenerator.Generate();
            repeated = await _reader.GetAsync(id);

            if (repeated.IsNone)
            {
                value.Id = id;
                return value;
            }
        }

        throw new InvalidOperationException($"No se pudo generar un identificador único después de {maxTries} intentos. Es posible que se hayan agotado las combinaciones disponibles.");
    }
}
