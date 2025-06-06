using NexusMods.Paths;
using NexusMods.Paths.Extensions;

namespace NexusMods.Hashing.xxHash3.Paths.Tests;

public class HashRelativePathTests
{
    [Fact]
    public void CanCreateAndCompareHashPaths()
    {
        var a = new HashRelativePath((Hash)0xDEADBEEFDECAFBAD, "foo/bar.pex");
        var b = new HashRelativePath((Hash)0x100000000000000F, "foo.pex");

        a.Should().BeEquivalentTo(a);
        a.Should().BeRankedEquallyTo(a);
        a.Should().NotBeRankedEquallyTo(b);

        (a == b).Should().BeFalse();
        (a != b).Should().BeTrue();

        a.Extension.Should().Be(new Extension(".pex"));
        a.FileName.Should().Be("bar.pex");
        a.GetHashCode().Should().Be(a.GetHashCode());
        b.GetHashCode().Should().NotBe(a.GetHashCode());
    }

    [Fact]
    public void CanConvertToString()
    {
        var a = new HashRelativePath((Hash)0xDEADBEEFDECAFBAD, "foo/bar.pex");
        var b = new HashRelativePath((Hash)0x100000000000000F, "foo.pex");

        a.ToString().Should().Be("0xDEADBEEFDECAFBAD|foo/bar.pex");
        b.ToString().Should().Be("0x100000000000000F|foo.pex");
    }
}
