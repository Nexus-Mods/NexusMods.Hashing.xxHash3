using NexusMods.Paths;

namespace NexusMods.Hashing.xxHash3.Paths.Tests;

public class HashTests
{
    private const string KnownString = "Something clever should go here";
    private static readonly Hash KnownHash = Hash.FromHex("b3a2155ccd08f233");
    private readonly IFileSystem _fileSystem = FileSystem.Shared;

    [Fact]
    public async Task CanHashFile()
    {
        var file = _fileSystem.GetKnownPath(KnownPath.CurrentDirectory).Combine($"tempFile{Guid.NewGuid()}");
        await file.WriteAllTextAsync(KnownString);
        (await file.MSxxHash3()).Should().Be(KnownHash); // sanity test against MSFT hasher.
        (await file.XxHash3Async()).Should().Be(KnownHash);

    }

    [Fact]
    public async Task CanHashLargeFile()
    {
        var file = _fileSystem.GetKnownPath(KnownPath.CurrentDirectory).Combine($"tempFile{Guid.NewGuid()}");
        var testArray = CreateTestArray();

        await file.WriteAllBytesAsync(testArray);
        (await file.MSxxHash3()).Should().Be(Hash.FromULong(0xABE1D1790E439680)); // sanity test against MSFT hasher.
        (await file.XxHash3Async()).Should().Be(Hash.FromULong(0xABE1D1790E439680));
        file.Delete();
    }

    [Fact]
    public async Task CanHashLargeFileWithReporting()
    {
        var file = _fileSystem.GetKnownPath(KnownPath.CurrentDirectory).Combine($"tempFile{Guid.NewGuid()}");
        var testArray = CreateTestArray();

        await file.WriteAllBytesAsync(testArray);

        var ms = new MemoryStream();
        var expectedHash = Hash.FromULong(0xABE1D1790E439680);

        (await file.XxHash3Async(reportFn: async m => await ms.WriteAsync(m))).Should().Be(expectedHash);
        (await file.MSxxHash3()).Should().Be(expectedHash);
        ms.ToArray().AsSpan().xxHash3().Should().Be(expectedHash);
        file.Delete();
    }

    private static byte[] CreateTestArray()
    {
        var emptyArray = new byte[1024 * 1024 * 10];
        for (var x = 0; x < emptyArray.Length; x++)
            emptyArray[x] = (byte)(x % 256);
        return emptyArray;
    }
}
