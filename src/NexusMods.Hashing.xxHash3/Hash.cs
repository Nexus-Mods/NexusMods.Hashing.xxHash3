using System;
using System.Buffers.Binary;
using JetBrains.Annotations;
using TransparentValueObjects;

namespace NexusMods.Hashing.xxHash3;

/// <summary>
/// Named object for an individual hash.
/// </summary>
[PublicAPI]
[ValueObject<ulong>]
public readonly partial struct Hash
{
    /// <summary>
    /// Represents a default [zero] value for the hash.
    /// Used for when the value is not set.
    /// </summary>
    public static readonly Hash Zero = From(0);

    /// <inheritdoc />
    public override string ToString()
    {
        return "0x" + ToHex();
    }

    /// <summary>
    /// Creates a hash from a long [8 byte] value.
    /// </summary>
    /// <param name="argHash">The hash to use.</param>
    public static Hash FromLong(in long argHash)
    {
        return From(unchecked((ulong)argHash));
    }

    /// <summary>
    /// Creates a hash from a ulong [8 byte] value.
    /// </summary>
    /// <param name="argHash">The hash to use.</param>
    public static Hash FromULong(in ulong argHash)
    {
        return From(argHash);
    }

    /// <summary>
    /// Converts hash from a hexadecimal string representation back into value.
    /// </summary>
    /// <param name="hash">The hash to convert.</param>
    public static Hash FromHex(string hash)
    {
        Span<byte> bytes = stackalloc byte[8];
        hash.FromHex(bytes);
        return From(BinaryPrimitives.ReadUInt64BigEndian(bytes));
    }

    /// <summary>
    /// Converts hash to hexadecimal string.
    /// </summary>
    public string ToHex()
    {
        Span<byte> buffer = stackalloc byte[8];
        BinaryPrimitives.WriteUInt64BigEndian(buffer, Value);
        return ((ReadOnlySpan<byte>)buffer).ToHex();
    }

    /// <summary>
    /// Converts a hash back to a long.
    /// </summary>
    public static implicit operator long(Hash a) => unchecked((long)a.Value);
}
