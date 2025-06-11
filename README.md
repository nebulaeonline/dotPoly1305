# dotPoly1305

A minimal, fast, cross-platform Poly1305 wrapper for .NET applications.

This library provides access to the Poly1305 message authentication code (MAC) algorithm, which is often used in conjunction with the ChaCha20 cipher for authenticated encryption. It is designed to be lightweight and efficient, making it suitable for high-performance applications that require secure message authentication.

The underlying optimized implementation is taken verbatim from BoringSSL maintained by Google, ensuring that it is both efficient and secure.

Tests are included and available in the Github repo.

[![NuGet](https://img.shields.io/nuget/v/nebulae.dotPoly1305.svg)](https://www.nuget.org/packages/nebulae.dotPoly1305)

---

## Features

- **Cross-platform**: Works on Windows, Linux, and macOS (x64 & Apple Silicon).
- **High performance**: Optimized for speed, leveraging native code SIMD-enabled code.
- **Easy to use**: Simple API for encryption and decryption.
- **Secure**: Uses BoringSSL's implementation, which is widely trusted in the industry.
- **Minimal dependencies**: No external dependencies required (all are included), making it lightweight and easy to integrate.

---

## Requirements

- .NET 8.0 or later
- AVX2 capable x86_64 _CPU (or Apple Silicon)
- Windows x64, Linux x64, or macOS (x64 & Apple Silicon)

## Usage

Poly1305 is a message authentication code (MAC) that can be used to verify the integrity and authenticity of a message. It is often used in conjunction with the ChaCha20 cipher for authenticated encryption.

Using the byte[] API:

```csharp

using Nebulae.Crypto;

Poly1305.Init(); // Initialize the Poly1305 context

byte[] key = new byte[32];       // One-time 32-byte key
byte[] message = Encoding.ASCII.GetBytes("hello world");
byte[] tag = new byte[16];       // 16-byte output tag

Poly1305.Auth(message, key, tag);

// 'tag' now contains the authentication tag
Console.WriteLine(Convert.ToHexString(tag));

```

Using the ReadOnlySpan<byte> API:

```csharp

using Nebulae.Crypto;

Poly1305.Init(); // Initialize the Poly1305 context

Span<byte> key = stackalloc byte[32];  // 32-byte one-time key
Span<byte> tag = stackalloc byte[16];  // 16-byte tag
ReadOnlySpan<byte> message = "authenticated stream data"u8;

Poly1305.Auth(message, key, tag);

// Output the tag
Console.WriteLine(Convert.ToHexString(tag));

```

---

## Installation

You can install the package via NuGet:

```bash

$ dotnet add package nebulae.dotPoly1305

```

Or via git:

```bash

$ git clone https://github.com/nebulaeonline/dotPoly1305.git
$ cd dotPoly1305
$ dotnet build

```

---

## License

MIT

## Roadmap

Unless there are vulnerabilities found, there are no plans to add any new features.