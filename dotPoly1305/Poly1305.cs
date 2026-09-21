using System.Security.Cryptography;

namespace nebulae.dotPoly1305;

public class Poly1305
{
    static Poly1305()
    {
        Poly1305Library.Init();
    }

    /// <summary>
    /// Computes a Poly1305 authentication tag for the specified message using the provided key.
    /// </summary>
    /// <remarks>Poly1305 is a cryptographic message authentication code (MAC) designed to verify the
    /// integrity and authenticity of a message. This method computes the authentication tag for the given message
    /// using the specified key and writes the result to the provided tag buffer.</remarks>
    /// <param name="message">The message to authenticate. This must be a non-null byte array containing the data to be processed.</param>
    /// <param name="key">The 32-byte key used for authentication. This must be a non-null byte array with a length of exactly 32
    /// bytes.</param>
    /// <param name="tag">A 16-byte buffer to store the computed authentication tag. This must be a non-null byte array with a length
    /// of exactly 16 bytes.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="message"/>, <paramref name="key"/>, or <paramref name="tag"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown if <paramref name="key"/> is not 32 bytes long or <paramref name="tag"/> is not 16 bytes long.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the underlying Poly1305 computation fails.</exception>
    public static void Auth(
        byte[] message,
        byte[] key,
        byte[] tag)
    {
        if (message is null)
            throw new ArgumentNullException(nameof(message));
        if (key is null)
            throw new ArgumentNullException(nameof(key));
        if (tag is null)
            throw new ArgumentNullException(nameof(tag));

        if (key.Length != 32)
            throw new ArgumentException("Poly1305 key must be 32 bytes.", nameof(key));
        if (tag.Length != 16)
            throw new ArgumentException("Poly1305 tag must be 16 bytes.", nameof(tag));

        int result = Poly1305Interop.poly1305_auth(tag, message, (UIntPtr)message.Length, key);
        if (result != 0)
            throw new InvalidOperationException($"poly1305_auth returned error {result}");
    }

    /// <summary>
    /// Verifies that the supplied Poly1305 tag matches the authentication tag computed for the message using the provided key.
    /// </summary>
    /// <param name="message">The message to authenticate.</param>
    /// <param name="key">The 32-byte authentication key.</param>
    /// <param name="expectedTag">The expected 16-byte Poly1305 tag.</param>
    /// <returns><see langword="true"/> when the tag matches; otherwise, <see langword="false"/>.</returns>
    public static bool Verify(
        byte[] message,
        byte[] key,
        byte[] expectedTag)
    {
        if (message is null)
            throw new ArgumentNullException(nameof(message));
        if (key is null)
            throw new ArgumentNullException(nameof(key));
        if (expectedTag is null)
            throw new ArgumentNullException(nameof(expectedTag));

        if (key.Length != 32)
            throw new ArgumentException("Poly1305 key must be 32 bytes.", nameof(key));
        if (expectedTag.Length != 16)
            throw new ArgumentException("Poly1305 tag must be 16 bytes.", nameof(expectedTag));

        byte[] actualTag = new byte[16];
        Auth(message, key, actualTag);
        return CryptographicOperations.FixedTimeEquals(actualTag, expectedTag);
    }

    /// <summary>
    /// Span-based Poly1305 authentication tag calculator for the specified message using the provided key.
    /// </summary>
    /// <remarks>Poly1305 is a cryptographic message authentication code (MAC) designed to verify the
    /// integrity and authenticity of a message. This method computes the authentication tag for the given message
    /// using the specified key and writes the result to the provided tag span.</remarks>
    /// <param name="message">The message to authenticate. This must be a read-only span of bytes containing the data to be processed.</param>
    /// <param name="key">The 32-byte key used for authentication. The key must be exactly 32 bytes long.</param>
    /// <param name="tag">A writable span of bytes where the computed 16-byte authentication tag will be stored. The span must be
    /// exactly 16 bytes long.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="key"/> is not 32 bytes long or if <paramref name="tag"/> is not 16 bytes long.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the underlying Poly1305 computation fails.</exception>
    public static void Auth(
    ReadOnlySpan<byte> message,
    ReadOnlySpan<byte> key,
    Span<byte> tag)
    {
        if (key.Length != 32)
            throw new ArgumentException("Poly1305 key must be 32 bytes.", nameof(key));
        if (tag.Length != 16)
            throw new ArgumentException("Poly1305 tag must be 16 bytes.", nameof(tag));

        unsafe
        {
            fixed (byte* m = message)
            fixed (byte* k = key)
            fixed (byte* t = tag)
            {
                int result = Poly1305Interop.poly1305_auth_ptr(
                    t, m, (UIntPtr)message.Length, k);
                if (result != 0)
                    throw new InvalidOperationException($"poly1305_auth_ptr returned error {result}");
            }
        }
    }

    /// <summary>
    /// Verifies that the supplied Poly1305 tag matches the authentication tag computed for the message using the provided key.
    /// </summary>
    /// <param name="message">The message to authenticate.</param>
    /// <param name="key">The 32-byte authentication key.</param>
    /// <param name="expectedTag">The expected 16-byte Poly1305 tag.</param>
    /// <returns><see langword="true"/> when the tag matches; otherwise, <see langword="false"/>.</returns>
    public static bool Verify(
        ReadOnlySpan<byte> message,
        ReadOnlySpan<byte> key,
        ReadOnlySpan<byte> expectedTag)
    {
        if (key.Length != 32)
            throw new ArgumentException("Poly1305 key must be 32 bytes.", nameof(key));
        if (expectedTag.Length != 16)
            throw new ArgumentException("Poly1305 tag must be 16 bytes.", nameof(expectedTag));

        Span<byte> actualTag = stackalloc byte[16];
        Auth(message, key, actualTag);
        return CryptographicOperations.FixedTimeEquals(actualTag, expectedTag);
    }
}
