namespace NexusMods.Hashing.xxHash3.Tests;

public class HashTests
{
    private const string KnownString = "Something clever should go here";
    private static readonly Hash KnownHash = Hash.FromHex("B3A2155CCD08F233");

    [Fact]
    public void CanConvertHashBetweenFormats()
    {
        var hash = Hash.FromULong(0xDEADBEEFDECAFBAD);

        ((ulong)hash).Should().Be(0xDEADBEEFDECAFBAD);
        hash.ToHex().Should().Be("DEADBEEFDECAFBAD");
        Hash.FromHex("DEADBEEFDECAFBAD").Should().Be(hash);

        Hash.FromLong(KnownHash).Should().Be(KnownHash);
        Hash.FromULong((ulong)KnownHash).Should().Be(KnownHash);
        KnownHash.ToString().Should().Be("0x" + KnownHash.ToHex());
    }

    [Fact]
    public void CanCompareHashes()
    {
        var hash1 = Hash.FromULong(0);
        var hash2 = Hash.FromULong(1);
        var hash3 = Hash.FromULong(2);

        hash1.CompareTo(hash2).Should().BeLessThan(0);
        hash2.CompareTo(hash3).Should().BeLessThan(0);

        hash1.Should().Be(hash1);
        hash1.Should().NotBe(hash2);

        hash1.Should().Be(hash1);

        Assert.True(hash1 != hash2);
        Assert.False(hash1 == hash2);
    }

    [Fact]
    public void CanHashStrings()
    {
        KnownString.xxHash3AsUtf8()
            .Should().Be(KnownHash);
    }

    [Fact]
    public async Task CanHashFile()
    {
        var file = Path.Combine(Environment.CurrentDirectory, $"tempFile_{nameof(CanHashFile)}_{Guid.NewGuid()}");
        await File.WriteAllTextAsync(file, KnownString);

        (await file.MSxxHash3()).Should().Be(KnownHash); // sanity test against MSFT hasher.
        (await file.xxHash3Async()).Should().Be(KnownHash);
    }

    [Fact]
    public async Task CanHashLargeFile()
    {
        var file = Path.Combine(Environment.CurrentDirectory, $"tempFile_{nameof(CanHashLargeFile)}_{Guid.NewGuid()}");
        var emptyArray = CreateTestArray();

        await File.WriteAllBytesAsync(file, emptyArray);
        (await file.MSxxHash3()).Should().Be(Hash.FromULong(0xABE1D1790E439680)); // sanity test against MSFT hasher.
        (await file.xxHash3Async()).Should().Be(Hash.FromULong(0xABE1D1790E439680));
        File.Delete(file);
    }

    [Fact]
    public async Task CanHashLargeFileWithReporting()
    {
        var file = Path.Combine(Environment.CurrentDirectory, $"tempFile_{nameof(CanHashLargeFileWithReporting)}_{Guid.NewGuid()}");
        var emptyArray = CreateTestArray();

        await File.WriteAllBytesAsync(file, emptyArray);
        var ms = new MemoryStream();
        var expectedHash = Hash.FromULong(0xABE1D1790E439680);

        await using var stream = new FileStream(file, FileMode.Open, FileAccess.Read);
        (await stream.xxHash3Async(reportFn: async m => await ms.WriteAsync(m))).Should().Be(expectedHash);
        (await file.MSxxHash3()).Should().Be(expectedHash);
        ms.ToArray().AsSpan().xxHash3().Should().Be(expectedHash);
        await stream.DisposeAsync();
        File.Delete(file);
    }

    private static byte[] CreateTestArray()
    {
        var emptyArray = new byte[1024 * 1024 * 10];
        for (var x = 0; x < emptyArray.Length; x++)
            emptyArray[x] = (byte)(x % 256);

        return emptyArray;
    }
}
