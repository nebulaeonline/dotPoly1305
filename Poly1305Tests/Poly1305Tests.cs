using nebulae.dotPoly1305;

namespace Poly1305Tests;

// https://datatracker.ietf.org/doc/html/rfc7539#section-2.5.2

public class Poly1305Tests
{
    [Fact]
    public void Auth_KnownVector_Rfc7539_ByteArray()
    {
        //Poly1305.Init();

        byte[] key = Convert.FromHexString(
            "85D6BE7857556D337F4452FE42D506A8" +
            "0103808AFB0DB2FD4ABFF6AF4149F51B");

        byte[] message = System.Text.Encoding.ASCII.GetBytes(
            "Cryptographic Forum Research Group");

        byte[] expectedTag = Convert.FromHexString(
            "A8061DC1305136C6C22B8BAF0C0127A9");

        byte[] tag = new byte[16];

        Poly1305.Auth(message, key, tag);
        Assert.Equal(expectedTag, tag);
    }

    [Fact]
    public void Auth_KnownVector_Rfc7539_Span()
    {
        //Poly1305.Init();
        
        Span<byte> key = Convert.FromHexString(
            "85D6BE7857556D337F4452FE42D506A8" +
            "0103808AFB0DB2FD4ABFF6AF4149F51B");

        ReadOnlySpan<byte> message = "Cryptographic Forum Research Group"u8;
        Span<byte> tag = stackalloc byte[16];

        Span<byte> expected = Convert.FromHexString(
            "A8061DC1305136C6C22B8BAF0C0127A9");

        Poly1305.Auth(message, key, tag);
        Assert.True(tag.SequenceEqual(expected));
    }

    [Fact]
    public void Verify_KnownVector_ReturnsTrue()
    {
        byte[] key = Convert.FromHexString(
            "85D6BE7857556D337F4452FE42D506A8" +
            "0103808AFB0DB2FD4ABFF6AF4149F51B");

        byte[] message = System.Text.Encoding.ASCII.GetBytes(
            "Cryptographic Forum Research Group");

        byte[] expectedTag = Convert.FromHexString(
            "A8061DC1305136C6C22B8BAF0C0127A9");

        Assert.True(Poly1305.Verify(message, key, expectedTag));
    }

    [Fact]
    public void Auth_NullArgument_ThrowsArgumentNullException()
    {
        byte[] message = [1, 2, 3];
        byte[] key = new byte[32];
        byte[] tag = new byte[16];

        Assert.Throws<ArgumentNullException>(() => Poly1305.Auth(null!, key, tag));
        Assert.Throws<ArgumentNullException>(() => Poly1305.Auth(message, null!, tag));
        Assert.Throws<ArgumentNullException>(() => Poly1305.Auth(message, key, null!));
    }
}