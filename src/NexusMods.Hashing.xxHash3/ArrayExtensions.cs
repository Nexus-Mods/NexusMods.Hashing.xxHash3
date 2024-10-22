using System;
using JetBrains.Annotations;

namespace NexusMods.Hashing.xxHash3;

/// <summary>
///     Extension methods tied to arrays.
/// </summary>
[PublicAPI]
public static class ArrayExtensions
{
    /// <summary>
    /// Hashes the given <see cref="Span{T}"/> of bytes using xxHash3.
    /// </summary>
    /// <param name="data">The data to hash.</param>
    /// <returns>Hash for the given data.</returns>
    public static Hash xxHash3(this byte[] data) => ((ReadOnlySpan<byte>)data).xxHash3();
}
