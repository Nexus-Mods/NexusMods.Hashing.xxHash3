using System.Buffers.Binary;
using System.IO.Hashing;
using NexusMods.Paths;

namespace NexusMods.Hashing.xxHash3.Paths.Tests;

/// <summary>
/// Contains various utility functions.
/// </summary>
public static class Utility
{
    /// <summary>
    /// Runs the microsoft's System.IO.Hashing algorithm returning in our format
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    internal static Hash MSHash(byte[] data)
    {
        var bytes = XxHash3.Hash(data);
        return Hash.From(BinaryPrimitives.ReadUInt64BigEndian(bytes));
    }

    /// <summary>
    /// Asynchronously hashes a given file.
    /// </summary>
    /// <param name="path">Path to the file to be hashed.</param>
    public static async Task<Hash> MSxxHash3(this AbsolutePath path)
    {
        var data = await path.ReadAllBytesAsync();
        return MSHash(data);
    }
}
