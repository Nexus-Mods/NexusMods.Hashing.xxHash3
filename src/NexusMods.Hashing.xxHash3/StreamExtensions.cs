using System;
using System.Buffers;
using System.IO;
using System.IO.Hashing;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace NexusMods.Hashing.xxHash3;

/// <summary>
/// Hashing related extensions tied to <see cref="Stream"/>(s).
/// </summary>
[PublicAPI]
public static class StreamExtensions
{
    /// <summary>
    /// Calculate the hash of a given stream starting at the current location of the stream
    /// </summary>
    /// <param name="stream">Source Stream</param>
    /// <param name="token">Allows you to cancel the operation.</param>
    /// <param name="reportFn">If not null, will be called with each block of data read from the input stream</param>
    public static async Task<Hash> xxHash3Async(this Stream stream, CancellationToken token = default,
        Func<Memory<byte>, Task>? reportFn = null)
    {
        return await stream.HashingCopyAsync(Stream.Null, token, reportFn);
    }

    /// <summary>
    /// Perform a stream copy, calculating the xxHash3 in the process
    /// </summary>
    /// <param name="inputStream">Where the input data will be read from.</param>
    /// <param name="outputStream">Where the output data will be placed.</param>
    /// <param name="token">Allows you to cancel the operation.</param>
    /// <param name="reportFn">If not null, will be called with each block of data read from the input stream</param>
    /// <returns></returns>
    public static async Task<Hash> HashingCopyAsync(this Stream inputStream, Stream outputStream,
        CancellationToken token, Func<Memory<byte>, Task>? reportFn = null)
    {
        using var rented = MemoryPool<byte>.Shared.Rent(1024 * 1024);
        var buffer = rented.Memory;

        var hasher = new XxHash3();

        var running = true;
        while (running && !token.IsCancellationRequested)
        {
            var read = await inputStream.ReadAsync(buffer, token);
            if (read == 0)
                break;

            var bounded = buffer[..read];

            if (reportFn != null)
                await reportFn(bounded);

            // Note: We can't deduplicate this code [without boxing] because ref param + async.
            // This code is based on the code inside xxHash3Algorithm
            var pendingWrite = outputStream.WriteAsync(bounded, token);
            hasher.Append(bounded.Span);
            await pendingWrite;
        }

        await outputStream.FlushAsync(token);
        return Hash.From(hasher.GetCurrentHashAsUInt64());
    }

    /// <summary>
    /// Perform a stream copy, calculating the xxHash3 inline with the copy routines. For each chunk
    /// of data read, call `fn` with a buffer of the data currently being processed.
    /// </summary>
    /// <param name="inputStream">The source stream</param>
    /// <param name="fn">Function to call with each chunk of data processed</param>
    /// <param name="token">Cancellation Token</param>
    /// <returns>The final cash.</returns>
    public static async Task<Hash> HashingCopyWithFnAsync(this Stream inputStream, Func<Memory<byte>, Task> fn,
        CancellationToken token = default)
    {
        return await inputStream.HashingCopyAsync(Stream.Null, token, fn);
    }
}
