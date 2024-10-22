using System;
using System.Threading;
using System.Threading.Tasks;
using NexusMods.Paths;

namespace NexusMods.Hashing.xxHash3.Paths;


/// <summary>
/// Extensions tied to <see cref="AbsolutePath"/>(s).
/// </summary>
public static class AbsolutePathExtensions
{
    /// <summary>
    /// Asynchronously hashes a given file.
    /// </summary>
    public static async Task<Hash> XxHash3Async(this AbsolutePath path, CancellationToken? token = null,
        Func<Memory<byte>, Task>? reportFn = null)
    {
        await using var stream = path.Read();
        return await stream.xxHash3Async(token ?? CancellationToken.None, reportFn);
    }
}
