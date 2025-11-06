namespace Application.Services;

public interface IDatabaseImporter
{
    Task RestoreAsync(Stream input);
}
