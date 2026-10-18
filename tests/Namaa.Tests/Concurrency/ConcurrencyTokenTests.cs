using Namaa.Application.Common.Concurrency;

namespace Namaa.Tests.Concurrency;

public sealed class ConcurrencyTokenTests
{
    public static TheoryData<string?> InvalidTokens => new()
    {
        null,
        string.Empty,
        "   ",
        "not-base64!",
        Convert.ToBase64String(new byte[1]),
        Convert.ToBase64String(new byte[7]),
        Convert.ToBase64String(new byte[9]),
        Convert.ToBase64String(new byte[64])
    };

    [Theory]
    [MemberData(nameof(InvalidTokens))]
    public void TryDecodeRejectsAnythingThatIsNotAnEightByteToken(string? value)
    {
        var success = ConcurrencyToken.TryDecode(value, out var decoded);

        Assert.False(success);
        Assert.Empty(decoded);
    }

    [Fact]
    public void TryDecodeAcceptsExactlyEightBytes()
    {
        byte[] expected = [0, 1, 2, 3, 4, 5, 6, 255];

        var success = ConcurrencyToken.TryDecode(
            Convert.ToBase64String(expected),
            out var decoded);

        Assert.True(success);
        Assert.Equal(expected, decoded);
    }
}
