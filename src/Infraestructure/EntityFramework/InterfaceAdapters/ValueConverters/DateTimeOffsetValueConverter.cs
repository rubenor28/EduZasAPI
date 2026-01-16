using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

public class DateTimeOffsetToDateTimeConverter : ValueConverter<DateTimeOffset, DateTime>
{
    public DateTimeOffsetToDateTimeConverter()
        : base(
            dfo => dfo.UtcDateTime, // Al guardar: Extrae UTC y convierte a DateTime
            dt => new DateTimeOffset(dt, TimeSpan.Zero) // Al leer: Crea DateTimeOffset con offset +0
        ) { }
}
