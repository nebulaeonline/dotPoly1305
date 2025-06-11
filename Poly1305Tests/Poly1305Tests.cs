using nebulae.dotPoly1305;

namespace Poly1305Tests
{
    // https://datatracker.ietf.org/doc/html/rfc7539#section-2.5.2

    public class Poly1305Tests
    {
        [Fact]
        public void Auth_KnownVector_Rfc7539_ByteArray()
        {
            Poly1305.Init();

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
            Poly1305.Init();
            
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
    }
}