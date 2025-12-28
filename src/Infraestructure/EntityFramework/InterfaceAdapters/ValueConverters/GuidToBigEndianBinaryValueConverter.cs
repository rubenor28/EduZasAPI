using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EntityFramework.InterfaceAdapters.ValueConverters;

public class GuidToBigEndianBinaryConverter : ValueConverter<Guid, byte[]>
{
    public GuidToBigEndianBinaryConverter()
        : base(
            // Al guardar: Guid -> byte[] (Big Endian para MariaDB)
            g => g.ToByteArray(true),
            // Al leer: byte[] -> Guid (Interpretando como Big Endian)
            b => new Guid(b, bigEndian: true)
        ) { }
}
