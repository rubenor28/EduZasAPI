using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

public class NullException(string? message = null) : Exception(message)
{
    public static void ThrowIfNull<T>(
        [NotNull] T? arg,
        [CallerArgumentExpression(nameof(arg))] string? message = null
    )
    {
        if (arg is null)
            throw new NullException(message);
    }
}
