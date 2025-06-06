namespace NexusMods.Hashing.xxHash3.Tests;

public class StreamExtensionTests
{
    private readonly byte[] _buffer;

    public StreamExtensionTests()
    {
        var random = new Random();
        _buffer = new byte[random.Next(1024 * 1024 * 2)];
        random.NextBytes(_buffer);
    }

    [Fact]
    public async Task CanHashStreams()
    {
        var stream = new MemoryStream(_buffer);
        var hashValue = await stream.xxHash3Async(CancellationToken.None);

        hashValue.Should().Be(Utility.MSHash(_buffer));
    }


    [Fact]
    public async Task CanHashStreamsWithCallback()
    {
        var stream = new MemoryStream(_buffer);
        var stream2 = new MemoryStream();
        var hashValue = await stream.HashingCopyWithFnAsync(async b =>
        {
            await stream2.WriteAsync(b);
        });

        hashValue.Should().Be(Utility.MSHash(_buffer));
        stream2.ToArray().Should().BeEquivalentTo(stream.ToArray());
    }
}
